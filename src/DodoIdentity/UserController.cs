using Common;
using Common.Extensions;
using Resources.Security;
using Dodo.Utility;
using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Common.Config;
using Microsoft.IdentityModel.Tokens;
using IdentityServer4.Services;
using IdentityServer4.Events;
using System.Security.Claims;
using System.Security.Principal;
using IdentityModel;

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

		private readonly IIdentityServerInteractionService _interaction;
		private readonly IAuthenticationSchemeProvider _schemeProvider;
		private readonly IEventService _events;

		public UserController(
			IIdentityServerInteractionService interaction,
			IAuthenticationSchemeProvider schemeProvider,
			IEventService events)
		{
			_interaction = interaction;
			_schemeProvider = schemeProvider;
			_events = events;
		}

		class LoginIdentity : IIdentity
		{
			public string AuthenticationType => AuthConstants.GUID;

			public bool IsAuthenticated => true;

			public string Name { get; private set; }

			public LoginIdentity(string username)
			{
				Name = username;
			}
		}


		[HttpPost(LOGIN)]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginModel login)
		{
			var user = ResourceManager.GetSingle(x => x.AuthData.Username == login.username);
			if (!user.AuthData.ChallengePassword(login.password, out var passphrase))
			{
				await _events.RaiseAsync(new UserLoginFailureEvent(login.username, "invalid credentials"));
				return BadRequest();
			}
			await _events.RaiseAsync(
				new UserLoginSuccessEvent(user.AuthData.Username, user.GUID.ToString(), user.Name));

			TemporaryTokenManager.SetTemporaryToken(passphrase, out var tokenKey, TimeSpan.FromHours(24));

			var id = new ClaimsIdentity(AuthConstants.AUTHSCHEME);
			id.AddClaim(new Claim(JwtClaimTypes.Subject, user.GUID.ToString()));
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

		[Authorize]
		public override Task<IActionResult> Get(Guid id)
		{
			return base.Get(id);
		}
	}
}