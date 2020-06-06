using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using Dodo.Users;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Resources.Security;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Common.Extensions;
using Dodo.Users.Tokens;

namespace Resources
{
	public abstract class CrudResourceAPIController<T, TSchema> : CustomController
		where T : DodoResource
		where TSchema : ResourceSchemaBase
	{
		protected CrudResourceServiceBase<T, TSchema> PublicService => 
			new CrudResourceServiceBase<T, TSchema>(Context, HttpContext, AuthService);
		protected abstract AuthorizationService<T, TSchema> AuthService { get; }

		[HttpPatch("{id}")]
		public virtual async Task<IActionResult> Update(Guid id, [FromBody]Dictionary<string, JsonElement> rawValues)
		{
			return (await PublicService.Update(id, rawValues)).ActionResult;
		}

		[HttpDelete("{id}")]
		public virtual async Task<IActionResult> Delete(Guid id)
		{
			return (await PublicService.Delete(id)).ActionResult;
		}

		[HttpGet("{id}")]
		public virtual async Task<IActionResult> Get(string id)
		{
			return (await PublicService.Get(id)).ActionResult;
		}

	}
}
