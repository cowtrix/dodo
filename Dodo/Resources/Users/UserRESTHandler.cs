using Common;
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
				IsEmailVerifyURL,
				WrapRawCall((req) => VerifyEmail(req))
				));
			base.AddRoutes(routeList);
		}

		private bool IsEmailVerifyURL(string url)
		{
			return url.StartsWith(VERIFY_PARAM);
		}

		public HttpResponse ResetPassword(HttpRequest request)
		{
			throw new NotImplementedException();
			if(DodoRESTServer.GetRequestOwner(request) != null)
			{
				return HttpBuilder.Forbidden();
			}
			var email = JsonExtensions.DeserializeAnonymousType(request.Content, new { Email = "" }).Email;
			var userWithEmail = ResourceUtility.GetManager<User>().GetSingle(x => x.Email == email);
			if(userWithEmail == null)
			{
				return HttpBuilder.NotFound();
			}
			userWithEmail.WebAuth.PasswordResetToken = WebPortalAuth.ResetToken.Generate();
			// TODO send an email with the auth link
			return HttpBuilder.OK();
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema("", "", "", "");
		}

		protected HttpResponse VerifyEmail(HttpRequest request)
		{
			var owner = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			if (owner == null)
			{
				throw HttpException.LOGIN;
			}
			if (owner.EmailVerified)
			{
				throw new HttpException("User email already verified", 200);
			}
			request.QueryParams.TryGetValue("token", out var verifyToken);
			var verification = owner.PushActions.FirstOrDefault(pa => pa is VerifyEmailAction) as VerifyEmailAction;
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
				var tempPushAction = user.PushActions.FirstOrDefault(pa => pa is TemporaryUserAction) as TemporaryUserAction;
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
