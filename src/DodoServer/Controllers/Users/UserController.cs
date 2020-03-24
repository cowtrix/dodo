using Common.Extensions;
using Resources.Security;
using Newtonsoft.Json;
using Resources;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Dodo.Users.Tokens;
using Common.Security;
using System.Linq;

namespace Dodo.Users
{
	[SecurityHeaders]
	[ApiController]
	[Route(RootURL)]
	public class UserController : ResourceController<User, UserSchema>
	{
		public const string RootURL = "auth";
		public const string LOGIN = "login";
		public const string LOGOUT = "logout";
		public const string REGISTER = "register";
		public const string RESET_PASSWORD = "resetpassword";
		public const string CHANGE_PASSWORD = "changepassword";
		public const string PARAM_TOKEN = "token";
		public const string VERIFY_EMAIL = "verifyemail";

		public class LoginModel
		{
			public string username { get; set; }
			public string password { get; set; }
			public string redirect { get; internal set; }
		}

		public class ChangePasswordModel
		{
			public string currentpassword { get; set; }
			public string newpassword { get; set; }
		}

		protected override AuthorizationManager<User, UserSchema> AuthManager => 
			new UserAuthManager(this.ControllerContext, Request);

		[HttpPost(LOGIN)]
		public async Task<IActionResult> Login([FromBody] LoginModel login)
		{
			if (Context.User != null)
			{
				// User is already logged in
				return Ok();
			}

			var user = ResourceManager.GetSingle(x => x.AuthData.Username == login.username);
			if(user == null)
			{
				return NotFound();
			}
			if (!user.AuthData.ChallengePassword(login.password, out var passphrase))
			{
				return BadRequest();
			}

			// Generate an encryption key that we will include in the cookie and throw away on our end
			var key = new Passphrase(KeyGenerator.GetUniqueKey(SessionToken.KEYSIZE));

			// Create the session token
			var token = new SessionToken(user, passphrase, key);
			using (var rscLock = new ResourceLock(user))
			{
				user = rscLock.Value as User;
				user.TokenCollection.Add(user, token);
				UserManager.Update(user, rscLock);
			}

			// Create the claims ID
			var id = new ClaimsIdentity(AuthConstants.AUTHSCHEME);
			id.AddClaim(new Claim(AuthConstants.SUBJECT, token.UserToken));
			id.AddClaim(new Claim(AuthConstants.KEY, key.Value));
			var principal = new ClaimsPrincipal(id);
			var props = new AuthenticationProperties
			{
				IsPersistent = true,
				ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(1))
			};
			// issue authentication cookie with subject ID and username
			await HttpContext.SignInAsync(AuthConstants.AUTHSCHEME, principal, props);

			return Ok();
		}

		[HttpGet(LOGOUT)]
		public async Task<IActionResult> Logout()
		{
			if(Context.User == null)
			{
				return Forbid();
			}
			await HttpContext.SignOutAsync(AuthConstants.AUTHSCHEME);
			using var rscLock = new ResourceLock(Context.User);
			var user = rscLock.Value as User;
			var session = user.TokenCollection.GetTokens<SessionToken>()
					.SingleOrDefault(t => t.UserToken == Context.UserToken);
			if(session == null)
			{
				return BadRequest();
			}
			user.TokenCollection.Remove(user, session);
			UserManager.Update(user, rscLock);
			return Ok();
		}


		[HttpGet(RESET_PASSWORD)]
		public async Task<IActionResult> RequestPasswordReset(string email)
		{
			if (Context.User != null && Context.User.PersonalData.Email != email)
			{
				return BadRequest("Mismatching emails");
			}
			var targetUser = UserManager.GetSingle(u => u.PersonalData.Email == email);
			if (targetUser != null)
			{
				using (var rscLock = new ResourceLock(targetUser))
				{
					targetUser = rscLock.Value as User;
					targetUser.TokenCollection.Add(targetUser, new ResetPasswordToken(targetUser));
					UserManager.Update(targetUser, rscLock);
				}
			}
			return Ok();
		}

		[HttpPost(RESET_PASSWORD)]
		public async Task<IActionResult> ResetPassword(string token, [FromBody]string password)
		{
			if(string.IsNullOrEmpty(token) || 
				!ValidationExtensions.IsStrongPassword(password, out _))
			{
				return BadRequest();
			}
			var user = UserManager.GetSingle(u => 
				u.TokenCollection.GetSingleToken<ResetPasswordToken>()?.TemporaryToken == token);
			if(user == null)
			{
				return BadRequest();
			}
			using(var rscLock = new ResourceLock(user))
			{
				user = rscLock.Value as User;
				user.TokenCollection.RemoveAll<ResetPasswordToken>(user);
				user.AuthData = new AuthorizationData(user.AuthData.Username, password);
				UserManager.Update(user, rscLock);
			}
			await Logout();
			return Redirect(DodoServer.DodoServer.Homepage);
		}

		[HttpPost(CHANGE_PASSWORD)]
		public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel model)
		{
			if(Context.User == null)
			{
				return Forbid();
			}
			if(!Context.User.AuthData.ChallengePassword(model.currentpassword, out _))
			{
				return Unauthorized();
			}
			using (var rscLock = new ResourceLock(Context.User))
			{
				var user = rscLock.Value as User;
				user.AuthData.ChangePassword(new Passphrase(model.currentpassword), new Passphrase(model.newpassword));
				UserManager.Update(user, rscLock);
			}
			await Logout();
			return Ok();
		}

		[HttpGet(VERIFY_EMAIL)]
		public async Task<IActionResult> VerifyEmail(string token)
		{
			if(Context.User == null)
			{
				return Forbid();
			}
			if(Context.User.PersonalData.EmailConfirmed)
			{
				return Ok();
			}
			var verifyToken = Context.User.TokenCollection.GetSingleToken<VerifyEmailToken>();
			if(verifyToken == null)
			{
				throw new Exception($"Verify token was null for user {Context.User.GUID}");
			}
			if(verifyToken.Token != token)
			{
				return BadRequest("Token mismatch");
			}
			using var rscLock = new ResourceLock(Context.User);
			var user = rscLock.Value as User;
			user.PersonalData.EmailConfirmed = true;
			UserManager.Update(user, rscLock);
			return Redirect(DodoServer.DodoServer.Homepage);
		}

		[HttpPost]
		[Route(REGISTER)]
		public override async Task<IActionResult> Create([FromBody] UserSchema schema)
		{
			var req = VerifyRequest(schema);
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			var user = ResourceManager.GetSingle(x => x.AuthData.Username == schema.Username || x.PersonalData.Email == schema.Email);
			if (user != null)
			{
				return Conflict();
			}
			var factory = ResourceUtility.GetFactory<User>();
			user = factory.CreateTypedObject(default(AccessContext), schema);
			var passphrase = new Passphrase(user.AuthData.PassPhrase.GetValue(schema.Password));
			return Ok(DodoJsonViewUtility.GenerateJsonView(user, EPermissionLevel.OWNER, user, passphrase));
		}
	}
}
