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
		public const string PARAM_TOKEN = "token";

		public class LoginModel
		{
			public string username { get; set; }
			public string password { get; set; }
		}

		protected override AuthorizationManager<User, UserSchema> AuthManager => 
			new UserAuthManager(this.ControllerContext, Request);

		[HttpPost(LOGIN)]
		public async Task<IActionResult> Login([FromBody] LoginModel login)
		{
			var user = ResourceManager.GetSingle(x => x.AuthData.Username == login.username);
			if (!user.AuthData.ChallengePassword(login.password, out var passphrase))
			{
				return BadRequest();
			}

			TemporaryTokenManager.SetTemporaryToken(user.GUID.ToString(), out var guidKey, TimeSpan.FromHours(24));
			TemporaryTokenManager.SetTemporaryToken(passphrase, out var passphraseKey, TimeSpan.FromHours(24));

			var id = new ClaimsIdentity(AuthConstants.AUTHSCHEME);
			id.AddClaim(new Claim(AuthConstants.SUBJECT, guidKey));
			id.AddClaim(new Claim(AuthConstants.KEY, passphraseKey));
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
			var context = User.GetContext();
			if(context.User == null)
			{
				return Forbid();
			}
			await HttpContext.SignOutAsync(AuthConstants.AUTHSCHEME);
			return Ok();
		}

		[HttpGet(RESET_PASSWORD)]
		public async Task<IActionResult> ResetPassword([FromBody]string password)
		{
			return Ok();
		}

		[HttpGet(RESET_PASSWORD)]
		public async Task<IActionResult> RequestPasswordReset(string email)
		{
			var context = User.GetContext();
			if(context.User != null)
			{
				var resetToken = 
			}
			return Ok();
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
			factory.CreateObject(default(AccessContext), schema);
			return Ok();
		}
	}
}
