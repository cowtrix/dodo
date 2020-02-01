using Common;
using Common.Extensions;
using REST.Security;
using Dodo.Utility;
using Newtonsoft.Json;
using REST;
using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;

namespace Dodo.Users
{
	[Route(RootURL)]
	public class UserController : ObjectRESTController<User, UserSchema>
	{
		public const string RootURL = "api/users";
		public const string CREATION_URL = "register";
		public const string VERIFY_PARAM = "verify";
		public const string RESETPASS_URL = "resetpassword";
		public const string CHANGEPASS_URL = "changepassword";

		[HttpPost("~/signin")]
		public async Task<ActionResult> SignIn(string provider, string returnUrl)
		{
			// Instruct the middleware corresponding to the requested external identity
			// provider to redirect the user agent to its own authorization endpoint.
			// Note: the authenticationScheme parameter must match the value configured in Startup.cs
			return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, provider);
		}

		[HttpGet("~/signout"), HttpPost("~/signout")]
		public ActionResult SignOut()
		{
			// Instruct the cookies middleware to delete the local cookie created
			// when the user agent is redirected from the external identity provider
			// after a successful authentication flow (e.g Google or Facebook).
			return SignOut("ServerCookie");
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
		protected IActionResult ResetPassword()
		{
			if (Request.Query.TryGetValue("token", out var token))
			{
				var context = Request.TryGetRequestOwner();
				var user = ResourceManager.
					GetSingle(u =>
					{
						var attr = u.PushActions.GetSinglePushAction<ResetPasswordAction>();
						if (attr == null)
						{
							return false;
						}
						return attr.TemporaryToken == token;
					});
				if (user == null)
				{
					throw HttpException.FORBIDDEN;
				}
				using (var rscLock = new ResourceLock(user))
				{
					if (context.User != null && user != context.User)
					{
						// A different user is trying to use someone else's reset token
						Logger.Error($"User {context.User.GUID} used user {user.GUID} password reset token");
						throw HttpException.FORBIDDEN;
					}
					var newPass = JsonConvert.DeserializeObject<string>(Request.ReadBody());
					if (user.WebAuth.ChallengePassword(newPass, out _))
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
				var email = JsonConvert.DeserializeObject<string>(Request.ReadBody());
				if (!ValidationExtensions.EmailIsValid(email))
				{
					throw new HttpException("Invalid email address", HttpStatusCode.BadRequest);
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

		protected IActionResult ChangePassword()
		{
			Request.GetAuth(out _, out var passRaw);
			var owner = Request.GetRequestOwner();
			using (var rscLock = new ResourceLock(owner.User))
			{
				var newPass = JsonConvert.DeserializeObject<string>(Request.ReadBody());
				var user = rscLock.Value as User;
				user.WebAuth.ChangePassword(new Passphrase(passRaw), new Passphrase(newPass));
				ResourceManager.Update(user, rscLock);
				return HttpBuilder.OK();
			}
		}

		protected IActionResult VerifyEmail()
		{
			var context = Request.TryGetRequestOwner();
			if (context.User == null)
			{
				return Unauthorized();
			}
			using (var rscLock = new ResourceLock(context.User))
			{
				var user = rscLock.Value as User;
				if (user.EmailVerified)
				{
					return BadRequest("User email already verified");
				}
				Request.Query.TryGetValue("token", out var verifyToken);
				var verification = user.PushActions.GetSinglePushAction<VerifyEmailAction>();
				if ((string.IsNullOrEmpty(verifyToken) || verifyToken == VERIFY_PARAM) && verification == null)
				{
					// We need to resend a verification email
					if (user.EmailVerified)
					{
						return BadRequest("User email already verified");
					}
					SendEmailVerificationEmail(user);
					ResourceManager.Update(user, rscLock);
					return Ok("Email Verification Sent");
				}
				if (verifyToken != verification.Token)
				{
					return BadRequest();
				}
				user.EmailVerified = true;
				ResourceManager.Update(user, rscLock);
				return Ok("Email verified");
			}
		}

		protected override IActionResult CreateInternal(UserSchema generalSchema)
		{
			//var schema = JsonConvert.DeserializeObject<UserSchema>(Request.ReadBody());
			var schema = generalSchema as UserSchema;
			var user = ResourceManager.GetSingle(u => u.Email == schema?.Email);
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
						SendEmailVerificationEmail(user);
						ResourceManager.Update(user, rscLock);
						return HttpBuilder.OK(user.GenerateJsonView(EPermissionLevel.USER, null, default));
					}
					else
					{
						// User is already registered with this email
						throw HttpException.CONFLICT;
					}
				}
			}
			return base.CreateInternal(generalSchema);
		}



		protected override void OnCreation(AccessContext context, User user)
		{
			SendEmailVerificationEmail(user);
		}

		void SendEmailVerificationEmail(User newUser)
		{
			var emailVerifyPushAction = new VerifyEmailAction();
			newUser.PushActions.Add(emailVerifyPushAction);
#if DEBUG
			Console.WriteLine($"Added a new VerifyEmailAction for user {newUser.WebAuth.Username}: {emailVerifyPushAction.Token}");
#endif
			EmailHelper.SendEmail(newUser.Email, newUser.Name, $"{Dodo.PRODUCT_NAME}: Please verify your email",
				"To verify your email, click the following link:\n" +
				$"{Dns.GetHostName()}/{VERIFY_PARAM}/{emailVerifyPushAction.Token}");
		}

		[HttpPost]
		public override IActionResult Create([FromBody] UserSchema schema)
		{
			return CreateInternal(schema);
		}
	}
}
