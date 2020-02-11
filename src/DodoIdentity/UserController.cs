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
using Microsoft.AspNetCore.Identity;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Dodo.Users
{
	
	[SecurityHeaders]
	[Authorize]
	[ApiController]
	public class UserController : ObjectRESTController<User, UserSchema>
	{
		public const string RootURL = "api/users";

		private readonly UserManager<User> _userManager;

		public UserController(UserManager<User> userManager, IAuthorizationService auth) : base(auth)
		{
			_userManager = userManager;
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("register")]
		public override async Task<IActionResult> Create([FromBody] UserSchema schema)
		{
			string error = null;
			if (schema == null || !schema.Verify(out error))
			{
				return BadRequest($"{error}\nExpecting application/json object:\n{JsonConvert.SerializeObject(new UserSchema(), Formatting.Indented)}");
			}
			var user = await _userManager.FindByNameAsync(schema.Username);
			if (user != null)
			{
				return Conflict();
			}
			var factory = ResourceUtility.GetFactory<User>();
			user = factory.CreateObject(schema);
			var result = await _userManager.CreateAsync(user, schema.Password);
			if (!result.Succeeded)
			{
				throw new Exception(result.Errors.First().Description);
			}
			return Ok();
		}

		[Authorize]
		public override Task<IActionResult> Get(Guid id)
		{
			return base.Get(id);
		}
	}
}