﻿using Common;
using Common.Extensions;
using Common.Security;
using Dodo.Resources;
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

		public HttpResponse ResetPassword(HttpRequest request)
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
						return attr.TemporaryToken.Value == token;
					});
				if(user == null)
				{
					throw HttpException.FORBIDDEN;
				}
				if (owner != null && user != owner)
				{
					// A different user is trying to use someone else's reset token
					Logger.Error($"User {owner.GUID} used user {user.GUID} password reset token");
					throw HttpException.FORBIDDEN;
				}
				var newPass = JsonConvert.DeserializeObject<string>(request.Content);
				if(user.WebAuth.Challenge(newPass, out _))
				{
					return HttpBuilder.ServerError("Cannot use same password.");
				}
				if(!ValidationExtensions.IsStrongPassword(newPass, out var error))
				{
					// Password does not meet requirements
					return HttpBuilder.ServerError(error);
				}
				user.WebAuth = new WebPortalAuth(user.WebAuth.Username, newPass);
				return HttpBuilder.OK("You've succesfully changed your password.");
			}
			else
			{
				var email = JsonConvert.DeserializeObject<string>(request.Content);
				if(!ValidationExtensions.EmailIsValid(email))
				{
					throw new HttpException("Invalid email address", 500);
				}
				var user = ResourceManager.GetSingle(u => u.Email == email);
				if(user != null)
				{
					try
					{
						user.PushActions.Add(new ResetPasswordAction(user));
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
			if (owner.EmailVerified)
			{
				throw new HttpException("User email already verified", 200);
			}
			request.QueryParams.TryGetValue("token", out var verifyToken);
			var verification = owner.PushActions.GetSinglePushAction<VerifyEmailAction>();
			if ((string.IsNullOrEmpty(verifyToken) || verifyToken == VERIFY_PARAM) && verification == null)
			{
				(ResourceManager as UserManager).SendEmailVerification(owner);
				return HttpBuilder.OK("Email Verification Sent");
			}
			if(verifyToken != verification.Token)
			{
				throw new HttpException("Incorrect verification code", 500);
			}
			owner.EmailVerified = true;
			return HttpBuilder.OK("Email verified");
		}

		protected override HttpResponse CreateObject(HttpRequest request)
		{
			var schema = JsonConvert.DeserializeObject<CreationSchema>(request.Content);
			var user = ResourceManager.GetSingle(u => u.Email == schema.Email);
			if(user != null)
			{
				var tempPushAction = user.PushActions.GetSinglePushAction<TemporaryUserAction>();
				if (tempPushAction != null)
				{
					// This is a temp user
					user.WebAuth.ChangePassword(new Passphrase(tempPushAction.TemporaryToken), new Passphrase(schema.Password));
					user.WebAuth.Username = schema.Username;
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
			return url == "register";
		}
	}
}
