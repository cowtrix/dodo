using Common;
using Common.Extensions;
using REST.Security;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using Dodo.Users;
using Dodo.Utility;

namespace REST
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
		protected IResourceManager<T> ResourceManager { get { return ResourceUtility.GetManager<T>(); } }

		protected bool IsAuthorised(AccessContext context, T target,
			EHTTPRequestType requestType, out EPermissionLevel permissionLevel)
		{
			if (target != null && !(target is T))
			{
				permissionLevel = EPermissionLevel.PUBLIC;
				return false;
			}
			if (target != null && context.User == null)
			{
				permissionLevel = EPermissionLevel.PUBLIC;
				if (requestType != EHTTPRequestType.GET)
				{
					return false; // Deny if not logged in and trying to do more than just fetch
				}
				return true; // If it's just GET then return a public view
			}
			if (target == null)
			{
				if (context.User != null)
				{
					if (typeof(T) == typeof(User) && requestType == EHTTPRequestType.POST)
					{
						// Special case, unregistered requesters can create new users
						permissionLevel = EPermissionLevel.OWNER;  // User is owner
						return true;
					}
					else if (requestType != EHTTPRequestType.GET)
					{
						permissionLevel = EPermissionLevel.PUBLIC;  // Requester not logged in, they can't make or patch stuff
						return false;
					}
					permissionLevel = EPermissionLevel.PUBLIC;
					return true;
				}
				else if (requestType == EHTTPRequestType.POST)
				{
					permissionLevel = EPermissionLevel.OWNER;
					return true;
				}
				permissionLevel = EPermissionLevel.PUBLIC;
				return true;
			}
			return target.IsAuthorised(context, requestType, out permissionLevel);
		}

		[HttpPost]
		public virtual IActionResult Create([FromBody]TSchema schema)
		{
			var context = Request.GetRequestOwner();
			if (!IsAuthorised(context, null, Request.MethodEnum(), out var permissionLevel))
			{
				return Forbid();
			}
			var factory = ResourceUtility.GetFactory<T>();
			T createdObject;
			try
			{
				/*var schema = JsonConvert.DeserializeObject(Request.ReadBody(), factory.SchemaType, new JsonSerializerSettings()
				{
					CheckAdditionalContent = true,
				}) as ResourceSchemaBase;*/
				createdObject = factory.CreateObject(schema);
				OnCreation(context, createdObject);
			}
			catch (Exception e)
			{
				return BadRequest($"Failed to deserialise JSON: {e.Message}");
			}
			return Ok(createdObject.GenerateJsonView(permissionLevel, context.User, context.Passphrase));
		}

		[HttpPatch("{id}")]
		public virtual IActionResult Update(Guid id, [FromBody]Dictionary<string, object> values)
		{
			var target = ResourceManager.GetSingle(rsc => rsc.GUID == id);
			if (target == null)
			{
				return NotFound();
			}
			var context = Request.GetRequestOwner();
			if (!IsAuthorised(context, target, Request.MethodEnum(), out var permissionLevel))
			{
				return Forbid();
			}
			using var resourceLock = new ResourceLock(target);
			target = resourceLock.Value as T;
			if (target == null)
			{
				return NotFound();
			}
			//var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(Request.ReadBody());
			if (values == null)
			{
				throw new HttpException("Invalid JSON body", HttpStatusCode.BadRequest);
			}
			var jsonSettings = new JsonSerializerSettings()
			{
				TypeNameHandling = TypeNameHandling.All
			};
			var prev = JsonConvert.SerializeObject(target, jsonSettings);
			target.PatchObject(values, permissionLevel, context.User, context.Passphrase);
			ResourceManager.Update(target, resourceLock);
			return Ok(target.GenerateJsonView(permissionLevel, context.User, context.Passphrase));
		}

		[HttpDelete("{id}")]
		public virtual IActionResult Delete(Guid id)
		{
			var target = ResourceManager.GetSingle(rsc => rsc.GUID == id);
			if (target == null)
			{
				return NotFound();
			}
			var context = Request.GetRequestOwner();
			if (!IsAuthorised(context, target, Request.MethodEnum(), out _))
			{
				return Forbid();
			}
			ResourceManager.Delete(target);
			return HttpBuilder.Custom("Resource deleted", System.Net.HttpStatusCode.OK);
		}

		[HttpGet("{id}")]
		public IActionResult Get(Guid id)
		{
			var target = ResourceManager.GetSingle(rsc => rsc.GUID == id);
			if (target == null)
			{
				return NotFound();
			}
			var context = Request.GetRequestOwner();
			if (!IsAuthorised(context, target, Request.MethodEnum(), out var permissionLevel))
			{
				return Forbid();
			}
			return Ok(target.GenerateJsonView(permissionLevel, context.User, context.Passphrase));
		}

		protected virtual void OnCreation(AccessContext context, T user)
		{
		}
	}
}
