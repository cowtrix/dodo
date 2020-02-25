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
	public class UserController : ObjectRESTController<User, UserSchema>
	{
		public const string RootURL = "auth";
		public const string LOGIN = "login";
		public const string LOGOUT = "logout";
		public const string REGISTER = "register";

		public class LoginModel
		{
			public string username { get; set; }
			public string password { get; set; }
		}

		private readonly IAuthenticationSchemeProvider _schemeProvider;

		public UserController(
			IAuthenticationSchemeProvider schemeProvider)
		{
			_schemeProvider = schemeProvider;
		}

		[HttpPost(LOGIN)]
		public async Task<IActionResult> Login([FromBody] LoginModel login)
		{
			var user = ResourceManager.GetSingle(x => x.AuthData.Username == login.username);
			if (!user.AuthData.ChallengePassword(login.password, out var passphrase))
			{
				return BadRequest();
			}
			TemporaryTokenManager.SetTemporaryToken(passphrase, out var tokenKey, TimeSpan.FromHours(24));

			var id = new ClaimsIdentity(AuthConstants.AUTHSCHEME);
			id.AddClaim(new Claim(AuthConstants.SUBJECT, user.GUID.ToString()));
			id.AddClaim(new Claim(AuthConstants.KEY, tokenKey));
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

		[HttpPost]
		[Route(REGISTER)]
		public override async Task<IActionResult> Create([FromBody] UserSchema schema)
		{
			string error = null;
			if (schema == null || !schema.Verify(out error))
			{
				return BadRequest($"{error}\nExpecting application/json object:\n{JsonConvert.SerializeObject(new UserSchema(), Formatting.Indented)}");
			}
			var user = ResourceManager.GetSingle(x => x.AuthData.Username == schema.Username);
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
