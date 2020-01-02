using Common;
using Common.Extensions;
using Common.Security;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleHttpServer.REST
{
	/// <summary>
	/// This class implements a basic REST handler that can create, get, update and delete a given object.
	/// This class handles receiving HTTP requests, transforming those requests into corresponding
	/// RESTful actions, and executing those actions on the objects.
	/// </summary>
	/// <typeparam name="T">The type of the resource</typeparam>
	public abstract class ObjectRESTHandler<T> : RESTHandler where T: class, IRESTResource
	{
		protected IResourceManager<T> ResourceManager { get { return ResourceUtility.GetManager<T>(); } }

		public override void AddRoutes(List<Route> routeList)
		{
			routeList.Add(new Route(
				$"{GetType().Name} POST",
				EHTTPRequestType.POST,
				URLIsCreation,
				WrapRawCall((req) => CreateObject(req))
				));
			routeList.Add(new Route(
				$"{GetType().Name} GET",
				EHTTPRequestType.GET,
				URLIsResource,
				WrapRawCall((req) => GetObject(req))
				));
			routeList.Add(new Route(
				$"{GetType().Name} PATCH",
				EHTTPRequestType.PATCH,
				URLIsResource,
				WrapRawCall((req) => UpdateObject(req))
				));
			routeList.Add(new Route(
				$"{GetType().Name} DELETE",
				EHTTPRequestType.DELETE,
				URLIsResource,
				WrapRawCall((req) => DeleteObject(req))
				));
		}

		/// <summary>
		/// Determine if a given URL points to a resource of type T
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		protected bool URLIsResource(string url)
		{
			return GetResource(url) != null;
		}

		/// <summary>
		/// Determine if a given URL is a link to create a resource of type T
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		protected abstract bool URLIsCreation(string url);

		/// <summary>
		/// Get the resource of this type given a url.
		/// </summary>
		/// <param name="url"></param>
		/// <returns>Null if the resource url is invalid or does not exist, or the given object of type T</returns>
		protected abstract T GetResource(string url);

		protected abstract string GetResourceURL(string url);

		/// <summary>
		/// Determine if the given request is authorized
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected abstract bool IsAuthorised(HttpRequest request, out EPermissionLevel visibility, out object context, out Passphrase passphrase);

		/// <summary>
		/// Create a new object, and return the resource url.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>If the creation schema is not correct, an example schema. If it is correct, the view of the object and it's new resource url.</returns>
		protected virtual HttpResponse CreateObject(HttpRequest request)
		{
			if(!IsAuthorised(request, out var view, out var context, out var password))
			{
				throw HttpException.FORBIDDEN;
			}
			var schema = GetCreationSchema();
			T createdObject = null;
			try
			{
				var creationInfo = JsonConvert.DeserializeObject(request.Content, schema.GetType(), new JsonSerializerSettings()
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

		/// <summary>
		/// Return an anonymous type representing the necessary information in the creation of this object
		/// </summary>
		/// <returns>An type with default values that specifies the schema type</returns>
		protected abstract IRESTResourceSchema GetCreationSchema();

		/// <summary>
		/// Create a new object of type T given the schema specified in this.GetCreationSchema()
		/// </summary>
		/// <param name="schema">The schema to use</param>
		/// <returns>An object of type T created with the given schema</returns>
		protected abstract T CreateFromSchema(HttpRequest request, IRESTResourceSchema schema);

		/// <summary>
		/// Update an object with a string->object dictionary, where the string is the name of the field
		/// and the object is the value to be set. For nested classes, the object should be deserialized
		/// as nested dictionaries.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>The view of the object that has been updated.</returns>
		protected virtual HttpResponse UpdateObject(HttpRequest request)
		{
			if (!IsAuthorised(request, out var view, out var context, out var passphrase))
			{
				throw HttpException.FORBIDDEN;
			}
			using (var resourceLock = new ResourceLock(GetResourceURL(request.Url)))
			{
				var target = resourceLock.Value;
				if (target == null)
				{
					throw HttpException.NOT_FOUND;
				}
				var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.Content);
				if (values == null)
				{
					throw new HttpException("Invalid JSON body", 400);
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

		protected virtual HttpResponse DeleteObject(HttpRequest request)
		{
			if (!IsAuthorised(request, out _, out _, out _))
			{
				throw HttpException.FORBIDDEN;
			}
			var target = GetResource(request.Url);
			if (target == null)
			{
				throw HttpException.NOT_FOUND;
			}
			DeleteObjectInternal(target);
			return HttpBuilder.Custom("Resource deleted", 204);
		}

		protected HttpResponse GetObject(HttpRequest request)
		{
			var target = GetResource(request.Url);
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
