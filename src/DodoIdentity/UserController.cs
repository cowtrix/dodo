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

namespace Dodo.Users
{
	[SecurityHeaders]
	[Authorize]
	[ApiController]
	[Route(RootURL)]
	public class UserController : ObjectRESTController<User, UserSchema>
	{
		public const string RootURL = "auth";
		public const string LOGIN = "login";
		public const string LOGOUT = "logout";
		public const string REGISTER = "register";

		[HttpPost(LOGIN)]
		[RequireHttps]
		public async Task<IActionResult> Login(string username, string password)
		{
			var user = ResourceManager.GetSingle(x => x.AuthData.Username == username);
			if(!user.AuthData.ChallengePassword(password, out var passphrase))
			{
				return BadRequest();
			}
			TemporaryTokenManager.SetTemporaryToken(passphrase, out var tokenKey, TimeSpan.FromHours(24));

			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthService.JwtKey));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
			var header = new JwtHeader(credentials);
			var payload = new JwtPayload
			{
				{ AuthService.GUID, user.GUID },
				{ AuthService.KEY, tokenKey }
			};
			var secToken = new JwtSecurityToken(header, payload);
			var handler = new JwtSecurityTokenHandler();
			var tokenString = handler.WriteToken(secToken);
			Response.Headers.Add(AuthService.JWTHEADER, tokenString);
			return Ok();
		}

		[HttpPost]
		[AllowAnonymous]
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