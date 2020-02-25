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
		protected virtual AuthorizationManager<T> AuthManager 
		{
			get
			{
				if(m_defaultHander == null)
				{
					m_defaultHander = new AuthorizationManager<T>();
				}
				return m_defaultHander;
			} 
		}
		private AuthorizationManager<T> m_defaultHander;
		
		protected IResourceManager<T> ResourceManager { get { return ResourceUtility.GetManager<T>(); } }

		public abstract Task<IActionResult> Create([FromBody] TSchema schema);

		protected virtual async Task<IActionResult> CreateInternal(TSchema schema)
		{
			LogRequest();
			var context = User.GetContext();
			if (!AuthManager.IsAuthorised(context, null, Request.MethodEnum(), out var permissionLevel))
			{
				return Forbid();
			}
			var factory = ResourceUtility.GetFactory<T>();
			T createdObject;
			try
			{
				createdObject = factory.CreateObject(context, schema) as T;
				OnCreation(context, createdObject);
			}
			catch (Exception e)
			{
				return BadRequest($"Failed to deserialise JSON: {e.Message}");
			}
			return Ok(createdObject.GenerateJsonView(permissionLevel, context.User, context.Passphrase));
		}

		[HttpPatch("{id}")]
		public virtual async Task<IActionResult> Update(Guid id, [FromBody]Dictionary<string, JsonElement> rawValues)
		{
			LogRequest();
			var target = ResourceManager.GetSingle(rsc => rsc.GUID == id);
			if (target == null)
			{
				return NotFound();
			}
			var context = User.GetContext();
			if (!AuthManager.IsAuthorised(context, target, Request.MethodEnum(), out var permissionLevel))
			{
				return Forbid();
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

			using (var resourceLock = new ResourceLock(target))
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
				target.PatchObject(values, permissionLevel, context.User, context.Passphrase);
				ResourceManager.Update(target, resourceLock);
			}
			return Ok(target.GenerateJsonView(permissionLevel, context.User, context.Passphrase));
		}

		[HttpDelete("{id}")]
		public virtual async Task<IActionResult> Delete(Guid id)
		{
			LogRequest();
			var target = ResourceManager.GetSingle(rsc => rsc.GUID == id);
			var context = User.GetContext();
			if (target == null)
			{
				return NotFound();
			}
			if (!AuthManager.IsAuthorised(context, target, Request.MethodEnum(), out _))
			{
				return Forbid();
			}
			ResourceManager.Delete(target);
			return Ok("Resource deleted");
		}

		[HttpGet("{id}")]
		public virtual async Task<IActionResult> Get(Guid id)
		{
			LogRequest();
			var target = ResourceManager.GetSingle(rsc => rsc.GUID == id);
			if (target == null)
			{
				return NotFound();
			}
			return Ok(target.GenerateJsonView(EPermissionLevel.PUBLIC, null, default));
		}

		protected void LogRequest()
		{
			Logger.Debug($"Received {Request.MethodEnum()} for {Request.Path}.");
		}

		protected virtual void OnCreation(AccessContext Context, T user)
		{
		}
	}
}
