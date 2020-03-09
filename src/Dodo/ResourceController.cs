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

namespace Resources
{

	/// <summary>
	/// This class implements a basic REST handler that can create, get, update and delete a given object.
	/// This class handles receiving HTTP requests, transforming those requests into corresponding
	/// RESTful actions, and executing those actions on the objects.
	/// </summary>
	/// <typeparam name="T">The type of the resource</typeparam>
	[ApiController]
	public abstract class ResourceController<T, TSchema> : CustomController<T, TSchema>
		where T : class, IDodoResource
		where TSchema : DodoResourceSchemaBase
	{
		public abstract Task<IActionResult> Create([FromBody] TSchema schema);

		protected virtual async Task<IActionResult> CreateInternal(TSchema schema)
		{
			var req = VerifyRequest(schema);
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			var factory = ResourceUtility.GetFactory<T>();
			T createdObject;
			try
			{
				createdObject = factory.CreateObject(req.Requester, schema) as T;
				if(req.Token != null && req.Requester.User != null)
				{
					// The user consumed a resource creation token to make this resource
					using(var rscLock = new ResourceLock(req.Requester.User))
					{
						var user = rscLock.Value as User;
						var token = user.TokenCollection.GetToken<ResourceCreationToken>(req.Token.GUID);
						if(token == null)
						{
							throw new Exception("Resource creation token was missing");
						}
						if(token.IsRedeemed)
						{
							throw new SecurityException($"Unexpected token consumption could indicate a user " +
								"is attempting to exploit creation of multiple resources.");
						}
						token.IsRedeemed = true;
						UserManager.Update(user, rscLock);
					}
				}
				OnCreation(req.Requester, createdObject);
			}
			catch (Exception e)
			{
				return BadRequest($"Failed to deserialise JSON: {e.Message}");
			}
			return Ok(createdObject.GenerateJsonView(req.PermissionLevel, req.Requester.User, req.Requester.Passphrase));
		}

		[HttpPatch("{id}")]
		public virtual async Task<IActionResult> Update(Guid id, [FromBody]Dictionary<string, JsonElement> rawValues)
		{
			var req = VerifyRequest(id);
			if(!req.IsSuccess)
			{
				return req.Error;
			}

			Dictionary<string, object> Flatten(Dictionary<string, JsonElement> jsonDict)
			{
				var result = new Dictionary<string, object>();
				foreach (var sub in jsonDict)
				{
					if (sub.Value.ValueKind == JsonValueKind.Object)
					{
						result[sub.Key] =
							Flatten(System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(sub.Value.GetRawText()));
					}
					else
					{
						result[sub.Key] = sub.Value.GetString();
					}
				}
				return result;
			}

			T target;
			using (var resourceLock = new ResourceLock(req.Resource))
			{
				target = resourceLock.Value as T;
				if (target == null)
				{
					return NotFound();
				}
				var values = Flatten(rawValues);
				if (values == null)
				{
					return BadRequest("Invalid JSON body");
				}
				var jsonSettings = new JsonSerializerSettings()
				{
					TypeNameHandling = TypeNameHandling.All
				};
				var prev = JsonConvert.SerializeObject(target, jsonSettings);
				target.PatchObject(values, req.PermissionLevel, req.Requester.User, req.Requester.Passphrase);
				ResourceManager.Update(target, resourceLock);
			}
			return Ok(target.GenerateJsonView(req.PermissionLevel, req.Requester.User, req.Requester.Passphrase));
		}

		[HttpDelete("{id}")]
		public virtual async Task<IActionResult> Delete(Guid id)
		{
			var req = VerifyRequest(id);
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			ResourceManager.Delete(req.Resource as T);
			return Ok("Resource deleted");
		}

		[HttpGet("{id}")]
		public virtual async Task<IActionResult> Get(Guid id)
		{
			var req = VerifyRequest(id);
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			return Ok(req.Resource.GenerateJsonView(req.PermissionLevel, req.Requester.User, req.Requester.Passphrase));
		}

		protected virtual void OnCreation(AccessContext Context, T user)
		{
		}
	}
}
