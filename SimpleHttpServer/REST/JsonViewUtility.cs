using Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleHttpServer.REST
{
	public class HTTPException : Exception
	{
		public static HTTPException UNAUTHORIZED { get { return new HTTPException("Unauthorised", 401); } }
		public static HTTPException FORBIDDEN { get { return new HTTPException("Forbidden", 403); } }
		public static HTTPException NOT_FOUND { get { return new HTTPException("Resource not found", 404); } }
		public static HTTPException CONFLICT { get { return new HTTPException("Conflict - resource may already exist", 409); } }
		public static Exception LOGIN { get { return new HTTPException("You need to login", 302); } }

		public readonly int ErrorCode;
		public HTTPException(string message, int errorCode) : base(message)
		{
			ErrorCode = errorCode;
		}
	}

	/// <summary>
	/// Fields and properties with this attribute will be serialized in REST api queries
	/// </summary>
	public class ViewAttribute : Attribute { }

	public static class JsonViewUtility
	{
		private static readonly HashSet<Type> m_explicitValueTypes = new HashSet<Type>()
		{
			typeof(string),
			typeof(Guid),
			typeof(GeoLocation)
		};

		/// <summary>
		/// This will generate a JSON object that represents viewable (public facing) properties of this object
		/// An object is marked as viewable with the ViewAttribute
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, object> GenerateJsonView(this object obj)
		{
			var vals = new Dictionary<string, object>();
			foreach (var prop in obj.GetType().GetProperties().Where(p => p.CanRead && p.GetCustomAttribute<ViewAttribute>() != null))
			{
				// Simple case, property is a primitive type
				if ((prop.PropertyType.IsValueType && prop.PropertyType.IsPrimitive)
					|| m_explicitValueTypes.Contains(prop.PropertyType))
				{
					vals.Add(prop.Name, prop.GetValue(obj));
				}
				else	// Object is a composite type (e.g. a struct or class) and so we recursively serialize it
				{
					vals.Add(prop.Name, prop.GetValue(obj).GenerateJsonView());
				}
			}
			foreach (var field in obj.GetType().GetFields().Where(p => p.GetCustomAttribute<ViewAttribute>() != null))
			{
				// Simple case, property is a primitive type
				if ((field.FieldType.IsValueType && field.FieldType.IsPrimitive)
					|| m_explicitValueTypes.Contains(field.FieldType))
				{
					vals.Add(field.Name, field.GetValue(obj));
				}
				else    // Object is a composite type (e.g. a struct or class) and so we recursively serialize it
				{
					vals.Add(field.Name, field.GetValue(obj).GenerateJsonView());
				}
			}
			return vals;
		}

		/// <summary>
		/// This will generate a JSON object that represents viewable (public facing) properties of this object
		/// An object is marked as viewable with the ViewAttribute
		/// </summary>
		/// <returns></returns>
		public static List<Dictionary<string, object>> GenerateJsonView<T>(this IEnumerable<T> obj)
		{
			return obj.Select(x => x.GenerateJsonView()).ToList();
		}
	}
}
