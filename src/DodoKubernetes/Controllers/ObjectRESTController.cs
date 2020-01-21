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
	public abstract class ObjectRESTController<T> : Controller where T: class, IDodoResource
	{
		protected IResourceManager<T> ResourceManager { get { return ResourceUtility.GetManager<T>(); } }
		private HashSet<string> m_resourceURLCache = new HashSet<string>();

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
					if (!CanCreateAtUrl(context, Request.Path, out var error))
					{
						return false;
					}
					return true;
				}
				permissionLevel = EPermissionLevel.PUBLIC;
				return true;
			}
			return target.IsAuthorised(context, requestType, out permissionLevel);
		}

		[HttpPost]
		protected virtual IActionResult CreateObject()
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
				var schema = JsonConvert.DeserializeObject(Request.ReadBody(), factory.SchemaType, new JsonSerializerSettings()
				{
					CheckAdditionalContent = true,
				}) as ResourceSchemaBase;
				createdObject = factory.CreateObject(schema);
				OnCreation(context, createdObject);
			}
			catch(Exception e)
			{
				return BadRequest($"Failed to deserialise JSON: {e.Message}");
			}
			return Ok(createdObject.GenerateJsonView(permissionLevel, context.User, context.Passphrase));
		}

		[HttpPatch]
		protected virtual IActionResult UpdateObject()
		{
			var target = ResourceUtility.GetResourceByURL(Request.Path) as T;
			if (target == null)
			{
				return NotFound();
			}
			var context = Request.GetRequestOwner();
			if (!IsAuthorised(context, target, Request.MethodEnum(), out var permissionLevel))
			{
				return Forbid();
			}
			using (var resourceLock = new ResourceLock(target))
			{
				target = resourceLock.Value as T;
				if (target == null)
				{
					return NotFound();
				}
				var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(Request.ReadBody());
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
				if(ResourceManager.Get(x => x.ResourceURL == target.ResourceURL && x.GUID != target.GUID).Any())
				{
					return Conflict();
				}
				ResourceManager.Update(target, resourceLock);
				return Ok(target.GenerateJsonView(permissionLevel, context.User, context.Passphrase));
			}
		}

		[HttpDelete]
		protected virtual IActionResult DeleteObject()
		{
			var target = ResourceUtility.GetResourceByURL(Request.Path) as T;
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
			lock(m_resourceURLCache)
			{
				m_resourceURLCache.Remove(target.ResourceURL);
			}
			return HttpBuilder.Custom("Resource deleted", System.Net.HttpStatusCode.OK);
		}

		[HttpGet]
		protected IActionResult GetObject()
		{
			var target = ResourceUtility.GetResourceByURL(Request.Path) as T;
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

		/// <summary>
		/// Get the parent resource from a ResourceURL
		/// E.g. /rebellions/myrebellion/wg/myworkinggroup will return the Rebellion "myrebellion"
		/// </summary>
		/// <param name="url"></param>
		/// <returns>The Group Resource that contains the given URL</returns>
		public ResourceReference<GroupResource> GetParentFromURL(string url)
		{
			var resource = ResourceUtility.GetResourceByURL(url) as GroupResource;
			if (resource == null)
			{
				return null;
			}
			return resource.Parent;
		}

		protected virtual bool CanCreateAtUrl(AccessContext context, string url, out string error)
		{
			var parent = GetParentFromURL(url);
			if (parent == null)
			{
				error = "Resource not found";
				return false;
			}
			if (context.User == null)
			{
				error = "You need to login";
				return false;
			}
			if (context.User.EmailVerified)
			{
				error = "You need to verify your email";
				return false;
			}
			error = null;
			return parent.Value.IsAdmin(context.User, context);
		}
	}
}
