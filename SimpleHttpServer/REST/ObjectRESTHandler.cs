using Common;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;

namespace SimpleHttpServer.REST
{
	public interface IRESTResource
	{
		string ResourceURL { get; }
	}

	public abstract class ObjectRESTHandler<T> : RESTHandler where T: class, IRESTResource
	{
		/// <summary>
		/// Get the resource of this type given a url.
		/// </summary>
		/// <param name="url"></param>
		/// <returns>Null if the resource url is invalid or does not exist, or the given object of type T</returns>
		protected abstract T GetResource(string url);

		/// <summary>
		/// Create a new object, and return the resource url.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>If the creation schema is not correct, an example schema. If it is correct, the view of the object and it's new resource url.</returns>
		protected virtual HttpResponse CreateObject(HttpRequest request)
		{
			if(GetResource(request.Url) != null)
			{
				throw HTTPException.CONFLICT;
			}
			var creationInfo = JsonExtensions.DeserializeAnonymousType(request.Content, GetCreationSchema());
			var createdObject = CreateFromSchema(creationInfo);
			return HttpBuilder.ResourceCreated(JsonViewUtility.GenerateJsonView(createdObject), createdObject.ResourceURL);
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
		protected abstract T CreateFromSchema(dynamic info);

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
			var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.Content);
			target.PatchObject(values);
			return HttpBuilder.OK(target.GenerateJsonView());
		}

		protected virtual HttpResponse DeleteObject(HttpRequest request)
		{
			var target = GetResource(request.Url);
			if (target == null)
			{
				throw HTTPException.NOT_FOUND;
			}
			DeleteObjectInternal(target);
			return HttpBuilder.OK();
		}
		protected abstract void DeleteObjectInternal(T target);
	}
}
