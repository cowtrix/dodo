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
	public abstract class PublicResourceAPIController<T, TSchema> : CustomController
		where T : DodoResource, IPublicResource
		where TSchema : ResourceSchemaBase
	{
		protected PublicResourceService<T, TSchema> PublicService => new PublicResourceService<T, TSchema>(Context, HttpContext);

		[HttpPatch("{id}")]
		public virtual async Task<IActionResult> Update(Guid id, [FromBody]Dictionary<string, JsonElement> rawValues)
		{
			return (await PublicService.Update(id, rawValues)).Result;
		}

		[HttpDelete("{id}")]
		public virtual async Task<IActionResult> Delete(Guid id)
		{
			return (await PublicService.Delete(id)).Result;
		}

		[HttpGet("{id}")]
		public virtual async Task<IActionResult> Get(string id)
		{
			return (await PublicService.Get(id)).Result;
		}

	}
}
