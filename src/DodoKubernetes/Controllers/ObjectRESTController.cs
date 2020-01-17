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

namespace REST
{
	/// <summary>
	/// This class implements a basic REST handler that can create, get, update and delete a given object.
	/// This class handles receiving HTTP requests, transforming those requests into corresponding
	/// RESTful actions, and executing those actions on the objects.
	/// </summary>
	/// <typeparam name="T">The type of the resource</typeparam>
	public abstract class ObjectRESTController<T> : Controller where T: class, IRESTResource
	{
		protected IResourceManager<T> ResourceManager { get { return ResourceUtility.GetManager<T>(); } }

		protected abstract bool IsAuthorised(HttpRequest request, out EPermissionLevel visibility, out object context, out Passphrase passphrase);

		[HttpPost]
		protected virtual IActionResult CreateObject(HttpRequest request)
		{
			if(!IsAuthorised(request, out var view, out var context, out var password))
			{
				throw HttpException.FORBIDDEN;
			}
			var factory = ResourceUtility.GetFactory<T>();
			var schema = GetCreationSchema();
			T createdObject = null;
			try
			{
				var creationInfo = JsonConvert.DeserializeObject(request.ReadBody(), schema.GetType(), new JsonSerializerSettings()
				{
					CheckAdditionalContent = true,
				});
				createdObject = CreateFromSchema(request, (IRESTResourceSchema)creationInfo);
				createdObject.Verify();
				ResourceUtility.Register(createdObject);
			}
			catch(Exception e)
			{
				throw new Exception($"Failed to deserialise JSON: {e.Message}\n Expected:\n {JsonConvert.SerializeObject(schema, Formatting.Indented)}");
			}
			return HttpBuilder.OK(createdObject.GenerateJsonView(view, context, password));
		}

		[HttpPatch]
		protected virtual IActionResult UpdateObject(HttpRequest request)
		{
			if (!IsAuthorised(request, out var view, out var context, out var passphrase))
			{
				throw HttpException.FORBIDDEN;
			}
			using (var resourceLock = new ResourceLock(ResourceUtility.GetResourceURL(request.Path)))
			{
				var target = resourceLock.Value;
				if (target == null)
				{
					throw HttpException.NOT_FOUND;
				}
				var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.ReadBody());
				if (values == null)
				{
					throw new HttpException("Invalid JSON body", HttpStatusCode.BadRequest);
				}
				var jsonSettings = new JsonSerializerSettings()
				{
					TypeNameHandling = TypeNameHandling.All
				};
				var prev = JsonConvert.SerializeObject(target, jsonSettings);
				target.PatchObject(values, view, context, passphrase);
				if(ResourceManager.Get(x => x.ResourceURL == target.ResourceURL && x.GUID != target.GUID).Any())
				{
					throw HttpException.CONFLICT;
				}
				ResourceManager.Update(target, resourceLock);
				return HttpBuilder.OK(target.GenerateJsonView(view, context, passphrase));
			}
		}

		[HttpDelete]
		protected virtual IActionResult DeleteObject(HttpRequest request)
		{
			if (!IsAuthorised(request, out _, out _, out _))
			{
				throw HttpException.FORBIDDEN;
			}
			var target = GetResource(request.Path);
			if (target == null)
			{
				throw HttpException.NOT_FOUND;
			}
			DeleteObjectInternal(target);
			lock(m_resourceURLCache)
			{
				m_resourceURLCache.Remove(target.ResourceURL);
			}
			return HttpBuilder.Custom("Resource deleted", System.Net.HttpStatusCode.OK);
		}

		[HttpGet]
		protected IActionResult GetObject(HttpRequest request)
		{
			var target = GetResource(request.Path);
			if (target == null)
			{
				throw HttpException.NOT_FOUND;
			}
			if (!IsAuthorised(request, out var view, out var context, out var passphrase))
			{
				throw HttpException.FORBIDDEN;
			}
			return HttpBuilder.OK(target.GenerateJsonView(view, context, passphrase));
		}

		protected abstract void DeleteObjectInternal(T target);
	}
}
