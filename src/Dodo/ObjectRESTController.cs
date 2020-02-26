using Common;
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

namespace Resources
{
	/// <summary>
	/// This class implements a basic REST handler that can create, get, update and delete a given object.
	/// This class handles receiving HTTP requests, transforming those requests into corresponding
	/// RESTful actions, and executing those actions on the objects.
	/// </summary>
	/// <typeparam name="T">The type of the resource</typeparam>
	[ApiController]
	public abstract class ObjectRESTController<T, TSchema> : Controller
		where T : class, IDodoResource
		where TSchema : DodoResourceSchemaBase
	{
		private DodoUserManager UserManager => ResourceUtility.GetManager<User>() as DodoUserManager;
		protected virtual AuthorizationManager<T> AuthManager => new AuthorizationManager<T>();
		
		protected IResourceManager<T> ResourceManager { get { return ResourceUtility.GetManager<T>(); } }

		public abstract Task<IActionResult> Create([FromBody] TSchema schema);

		protected virtual async Task<IActionResult> CreateInternal(TSchema schema)
		{
			var req = VerifyRequest();
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			var factory = ResourceUtility.GetFactory<T>();
			T createdObject;
			try
			{
				createdObject = factory.CreateObject(req.Requester, schema) as T;
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
			return Ok(req.Resource.GenerateJsonView(EPermissionLevel.PUBLIC, null, default));
		}

		protected void LogRequest()
		{
			Logger.Debug($"Received {Request.MethodEnum()} for {Request.Path}.");
		}

		protected virtual void OnCreation(AccessContext Context, T user)
		{
		}

		protected ResourceRequest VerifyRequest(Guid id = default)
		{
			LogRequest();
			var target = ResourceManager.GetSingle(rsc => rsc.GUID == id);
			if (target == null)
			{
				return new ResourceRequest(NotFound());
			}
			var context = User.GetContext();
			return AuthManager.IsAuthorised(context, target, Request.MethodEnum());
		}
	}
}
