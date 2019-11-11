using Common;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;

namespace SimpleHttpServer.REST
{
	public abstract class ObjectRESTHandler<T> : RESTHandler where T: class, IRESTResource
	{
		/// <summary>
		/// Get the resource of this type given a url.
		/// </summary>
		/// <param name="url"></param>
		/// <returns>Null if the resource url is invalid or does not exist, or the given object of type T</returns>
		protected abstract T GetResource(string url);

		/// <summary>
		/// Determine if the given request is authorized
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		protected abstract bool IsAuthorised(HttpRequest request, out EPermissionLevel visibility, out object contxt, out string passphrase);

		/// <summary>
		/// Create a new object, and return the resource url.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>If the creation schema is not correct, an example schema. If it is correct, the view of the object and it's new resource url.</returns>
		protected virtual HttpResponse CreateObject(HttpRequest request)
		{
			if(!IsAuthorised(request, out var view, out var context, out var password))
			{
				throw HTTPException.FORBIDDEN;
			}
			if(GetResource(request.Url) != null)
			{
				throw HTTPException.CONFLICT;
			}
			var schema = GetCreationSchema();
			T createdObject = null;
			try
			{
				var creationInfo = JsonExtensions.DeserializeAnonymousType(request.Content, schema);
				createdObject = CreateFromSchema(request, creationInfo);
			}
			catch(NullReferenceException e)
			{
				throw new Exception($"Failed to deserialise JSON. Expected:\n {JsonConvert.SerializeObject(schema, Formatting.Indented)}");
			}
			catch(RuntimeBinderException e)
			{
				throw new Exception($"Failed to deserialise JSON. Expected:\n {JsonConvert.SerializeObject(schema, Formatting.Indented)}");
			}
			return HttpBuilder.OK(createdObject.GenerateJsonView(view, context, password));
		}

		/// <summary>
		/// Return an anonymous type representing the necessary information in the creation of this object
		/// </summary>
		/// <returns>An anonymous type with default values that specifies the schema type</returns>
		protected abstract dynamic GetCreationSchema();
		/// <summary>
		/// Create a new object of type T given the schema specified in this.GetCreationSchema()
		/// </summary>
		/// <param name="info">The schema to use</param>
		/// <returns>An object of type T created with the given schema</returns>
		protected abstract T CreateFromSchema(HttpRequest request, dynamic info);
		/// <summary>
		/// Update an object with a string->object dictionary, where the string is the name of the field
		/// and the object is the value to be set. For nested classes, the object should be deserialized
		/// as nested dictionaries.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>The view of the object that has been updated.</returns>
		protected virtual HttpResponse UpdateObject(HttpRequest request)
		{
			var target = GetResource(request.Url);
			if(target == null)
			{
				throw HTTPException.NOT_FOUND;
			}
			if (!IsAuthorised(request, out var view, out var context, out var passphrase))
			{
				throw HTTPException.FORBIDDEN;
			}
			var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.Content);
			if(values == null)
			{
				throw new HTTPException("Invalid JSON body", 400);
			}
			target.PatchObject(values, view, context, passphrase);
			return HttpBuilder.OK(target.GenerateJsonView(view, context, passphrase));
		}
		protected virtual HttpResponse DeleteObject(HttpRequest request)
		{
			if (!IsAuthorised(request, out _, out _, out _))
			{
				throw HTTPException.FORBIDDEN;
			}
			var target = GetResource(request.Url);
			if (target == null)
			{
				throw HTTPException.NOT_FOUND;
			}
			DeleteObjectInternal(target);
			return HttpBuilder.Custom("Resource deleted", 204);
		}
		protected abstract void DeleteObjectInternal(T target);
	}
}
