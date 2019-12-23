using Common;
using Common.Extensions;
using Common.Security;
using Dodo.Resources;
using Dodo.Utility;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dodo.Users
{
	public class UserRESTHandler : DodoRESTHandler<User>
	{
		public const string CREATION_URL = "register";
		public const string VERIFY_PARAM = "verify";
		public const string RESETPASS_URL = "resetpassword";

		protected override string CreationPostfix => CREATION_URL;

		public class CreationSchema : IRESTResourceSchema
		{
			public string Username = "";
			public string Password = "";
			public string Name = "";
			public string Email = "";

			public CreationSchema(string username, string password, string name, string email)
			{
				Username = username;
				Password = password;
				Name = name;
				Email = email;
			}
		}

		public override void AddRoutes(List<Route> routeList)
		{
			routeList.Add(new Route(
				$"User Email Verification",
				EHTTPRequestType.POST,
				url => url.StartsWith(VERIFY_PARAM),
				WrapRawCall((req) => VerifyEmail(req))
				));
			routeList.Add(new Route(
				$"User Password Reset",
				EHTTPRequestType.POST,
				url => url.StartsWith(RESETPASS_URL),
				WrapRawCall((req) => ResetPassword(req))
				));
			base.AddRoutes(routeList);
		}

		/// <summary>
		/// This handles resetting a user's password.
		/// If the request is passed without parameters, it expects an email to be provided.
		/// If a user with that email exists, they will receive an email with a password
		/// reset link (which is back to this handler, but with a token parameter).
		/// If they pass this page a valid token, then they are able to provide a new
		/// password in the request content.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected HttpResponse ResetPassword(HttpRequest request)
		{
			if(request.QueryParams.TryGetValue("token", out var token))
			{
				var owner = DodoRESTServer.TryGetRequestOwner(request, out _);
				var user = ResourceManager.
					GetSingle(u =>
					{
						var attr = u.PushActions.GetSinglePushAction<ResetPasswordAction>();
						if(attr == null)
						{
							return false;
						}
						return attr.TemporaryToken == token;
					});
				if(user == null)
				{
					throw HttpException.FORBIDDEN;
				}
				using (var rscLock = new ResourceLock(user))
				{
					if (owner != null && user != owner)
					{
						// A different user is trying to use someone else's reset token
						Logger.Error($"User {owner.GUID} used user {user.GUID} password reset token");
						throw HttpException.FORBIDDEN;
					}
					var newPass = JsonConvert.DeserializeObject<string>(request.Content);
					if (user.WebAuth.Challenge(newPass, out _))
					{
						return HttpBuilder.ServerError("Cannot use same password.");
					}
					if (!ValidationExtensions.IsStrongPassword(newPass, out var error))
					{
						// Password does not meet requirements
						return HttpBuilder.ServerError(error);
					}
					user.WebAuth = new WebPortalAuth(user.WebAuth.Username, newPass);
					ResourceManager.Update(user, rscLock);
					return HttpBuilder.OK("You've succesfully changed your password.");
				}
			}
			else
			{
				var email = JsonConvert.DeserializeObject<string>(request.Content);
				if(!ValidationExtensions.EmailIsValid(email))
				{
					throw new HttpException("Invalid email address", 500);
				}
				var user = ResourceManager.GetSingle(u => u.Email == email);
				if (user != null)
				{
					try
					{
						using (var rscLock = new ResourceLock(user))
						{
							user.PushActions.Add(new ResetPasswordAction(user));
							ResourceManager.Update(user, rscLock);
						}
					}
					catch (PushActionDuplicateException)
					{
					}
				}
				return HttpBuilder.OK("If an account with that email exists, a password reset email has been sent");
			}
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema("", "", "", "");
		}

		protected HttpResponse VerifyEmail(HttpRequest request)
		{
			var owner = DodoRESTServer.TryGetRequestOwner(request, out var passphrase);
			if (owner == null)
			{
				throw HttpException.LOGIN;
			}
			using (var rscLock = new ResourceLock(owner))
			{
				if (owner.EmailVerified)
				{
					throw new HttpException("User email already verified", 200);
				}
				request.QueryParams.TryGetValue("token", out var verifyToken);
				var verification = owner.PushActions.GetSinglePushAction<VerifyEmailAction>();
				if ((string.IsNullOrEmpty(verifyToken) || verifyToken == VERIFY_PARAM) && verification == null)
				{
					if (owner.EmailVerified)
					{
						throw new HttpException("User email already verified", 200);
					}
					var emailVerifyPushAction = new VerifyEmailAction(owner);
					owner.PushActions.Add(emailVerifyPushAction);
					EmailHelper.SendEmail(owner.Email, owner.Name, $"{DodoServer.PRODUCT_NAME}: Please verify your email",
						"To verify your email, click the following link:\n" +
						$"{DodoServer.GetURL()}/{owner.ResourceURL}?verify={emailVerifyPushAction.Token}");
					ResourceManager.Update(owner, rscLock);
					return HttpBuilder.OK("Email Verification Sent");
				}
				if (verifyToken != verification.Token)
				{
					throw new HttpException("Incorrect verification code", 500);
				}
				owner.EmailVerified = true;
				ResourceManager.Update(owner, rscLock);
				return HttpBuilder.OK("Email verified");
			}
		}

		protected override HttpResponse CreateObject(HttpRequest request)
		{
			var schema = JsonConvert.DeserializeObject<CreationSchema>(request.Content);
			var user = ResourceManager.GetSingle(u => u.Email == schema.Email);
			using (var rscLock = new ResourceLock(user))
			{
				if (user != null)
				{
					var tempPushAction = user.PushActions.GetSinglePushAction<TemporaryUserAction>();
					if (tempPushAction != null)
					{
						// This is a temp user
						user.WebAuth.ChangePassword(new Passphrase(tempPushAction.TemporaryToken), new Passphrase(schema.Password));
						user.WebAuth.Username = schema.Username;
						ResourceManager.Update(user, rscLock);
						return HttpBuilder.OK(user.GenerateJsonView(EPermissionLevel.USER, null, default));
					}
					else
					{
						// User is already registered with this email
						throw HttpException.CONFLICT;
					}
				}
				return base.CreateObject(request);
			}
		}

		protected override User CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var newUser = new User(info);
			if (URLIsCreation(newUser.ResourceURL))
			{
				throw new Exception("Reserved Resource URL");
			}
			newUser.Verify();
			return newUser;
		}

		protected override bool URLIsCreation(string url)
		{
			return url == CREATION_URL;
		}
	}
}
