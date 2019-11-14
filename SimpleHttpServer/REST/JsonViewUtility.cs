using Common;
using Common.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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

	public enum EPermissionLevel : byte
	{
		PUBLIC = 0,	// Any requester
		USER = 1,	// A valid, signed in user
		ADMIN = 2,	// An administrator of the resource
		OWNER = 3,	// An owner of the resource
		SYSTEM = byte.MaxValue,
	}

	/// <summary>
	/// Fields and properties with this attribute will be serialized in REST api queries.
	/// </summary>
	public class ViewAttribute : Attribute {
		public EPermissionLevel ViewPermission { get; private set; }
		public EPermissionLevel EditPermission { get; private set; }
		public ViewAttribute(EPermissionLevel viewPermission)
		{
			ViewPermission = viewPermission;
			EditPermission = viewPermission;
		}

		public ViewAttribute(EPermissionLevel viewPermission, EPermissionLevel editPermission)
		{
			ViewPermission = viewPermission;
			EditPermission = editPermission;
		}
	}

	public static class JsonViewUtility
	{
		private static readonly HashSet<Type> m_explicitValueTypes = new HashSet<Type>()
		{
			typeof(string),
			typeof(Guid),
			typeof(GeoLocation)
		};

		/// <summary>
		/// This will generate a JSON object that represents viewable (public facing) properties of this object.
		/// An object is marked as viewable with the ViewAttribute
		/// </summary>
		/// <returns>A string/object dictionary where the string value is the name of a field and the object is its value</returns>
		public static Dictionary<string, object> GenerateJsonView(this object obj, EPermissionLevel visibility, object requester, string passPhrase, [CallerMemberName]string memberName = "")
		{
			if(obj == null)
			{
				return null;
			}
			var vals = new Dictionary<string, object>();
			if (memberName != "GenerateJsonView")
			{
				vals.Add("Permission", visibility);
			}
			foreach (var prop in obj.GetType().GetProperties().Where(p => p.CanRead))
			{
				var attr = prop.GetCustomAttribute<ViewAttribute>();
				if (attr == null || attr.ViewPermission > visibility)
				{
					continue;
				}
				// Simple case, property is a primitive type
				if ((prop.PropertyType.IsValueType && prop.PropertyType.IsPrimitive)
					|| m_explicitValueTypes.Contains(prop.PropertyType))
				{
					vals.Add(prop.Name, prop.GetValue(obj));
				}
				else if(typeof(IDecryptable).IsAssignableFrom(prop.PropertyType))
				{
					var decryptable = prop.GetValue(obj) as IDecryptable;
					if(!decryptable.TryGetValue(requester, passPhrase, out var data))
					{
						continue;
					}
					vals.Add(prop.Name, data.GenerateJsonView(visibility, requester, passPhrase));
				}
				else	// Object is a composite type (e.g. a struct or class) and so we recursively serialize it
				{
					vals.Add(prop.Name, prop.GetValue(obj).GenerateJsonView(visibility, requester, passPhrase));
				}
			}
			foreach (var field in obj.GetType().GetFields())
			{
				var attr = field.GetCustomAttribute<ViewAttribute>();
				if (attr == null || attr.ViewPermission > visibility)
				{
					continue;
				}
				// Simple case, property is a primitive type
				if ((field.FieldType.IsValueType && field.FieldType.IsPrimitive)
					|| m_explicitValueTypes.Contains(field.FieldType))
				{
					vals.Add(field.Name, field.GetValue(obj));
				}
				else if (typeof(IDecryptable).IsAssignableFrom(field.FieldType))
				{
					var decryptable = field.GetValue(obj) as IDecryptable;
					if (!decryptable.TryGetValue(requester, passPhrase, out var data))
					{
						continue;
					}
					vals.Add(field.Name, data.GenerateJsonView(visibility, requester, passPhrase));
				}
				else    // Object is a composite type (e.g. a struct or class) and so we recursively serialize it
				{
					vals.Add(field.Name, field.GetValue(obj).GenerateJsonView(visibility, requester, passPhrase));
				}
			}
			return vals;
		}

		/// <summary>
		/// This will generate a JSON object that represents viewable (public facing) properties of this object
		/// An object is marked as viewable with the ViewAttribute
		/// </summary>
		/// <returns></returns>
		public static List<Dictionary<string, object>> GenerateJsonView<T>(this IEnumerable<T> obj,
			EPermissionLevel visibility, object requester, string passPhrase)
		{
			return obj.Select(x => x.GenerateJsonView(visibility, requester, passPhrase)).ToList();
		}

		/// <summary>
		/// Given a string/object dictionary, where the string is the name of a field or property and the
		/// object is the value to be set, patch the values of a target object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="targetObject"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static T PatchObject<T>(this T targetObject, Dictionary<string, object> values, EPermissionLevel visibility,
			object requester, string passphrase)
		{
			var targetType = targetObject != null ? targetObject.GetType() : typeof(T);
			if ((targetType.IsValueType && targetType.IsPrimitive)
					|| m_explicitValueTypes.Contains(targetType))
			{
				return (T)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(values), targetType);
			}
			if (targetObject is IDecryptable)
			{
				var decryptable = targetObject as IDecryptable;
				if (!decryptable.TryGetValue(requester, passphrase, out var encryptedObject))
				{
					return targetObject;
				}
				encryptedObject.PatchObject(values, visibility, requester, passphrase);
				decryptable.SetValue(encryptedObject, visibility, requester, passphrase);
				return targetObject;
			}
			var targetProperties = targetObject.GetType().GetProperties();
			var targetFields = targetObject.GetType().GetFields();

			var validFields = values.Select(kvp => targetProperties.FirstOrDefault(x => x.Name == kvp.Key) as MemberInfo ??
				targetFields.FirstOrDefault(x => x.Name == kvp.Key) as MemberInfo)
				.Where(member => member != null);
			if(validFields.Count() != values.Count)
			{
				throw new Exception("Invalid field names");
			}
			if(validFields.Where(member => member.GetCustomAttribute<ViewAttribute>()?.EditPermission <= visibility).Count() != values.Count)
			{
				throw new Exception("Insufficient privileges");
			}

			foreach (var val in values)
			{
				var targetMember = targetProperties.FirstOrDefault(x => x.Name == val.Key);
				if (targetMember == null)
				{
					continue;
				}
				if (targetMember.GetCustomAttribute<NoPatchAttribute>() != null)
				{
					throw new Exception($"Cannot patch field {targetMember.Name}: NoPatch");
				}
				var viewAttr = targetMember.GetCustomAttribute<ViewAttribute>();
				if(viewAttr == null || viewAttr.EditPermission > visibility)
				{
					throw new Exception($"Cannot patch field {targetMember.Name}: Insufficient privileges");
				}
				object objToPatch = targetObject;
				var value = targetMember.GetValue(targetObject);
				var fieldValue = value;
				var valueToSet = val.Value;
				if (typeof(IDecryptable).IsAssignableFrom(targetMember.PropertyType) &&
					(value == null || !(value as IDecryptable).TryGetValue(requester, passphrase, out value)))
				{
					// Auth is incorrect
					continue;
				}
				try
				{
					var subValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(val.Value.ToString());
					valueToSet = value.PatchObject(subValues, visibility, requester, passphrase);
				}
				catch
				{
				}
				if (typeof(IDecryptable).IsAssignableFrom(targetMember.PropertyType))
				{
					var decryptable = fieldValue as IDecryptable;
					decryptable.SetValue(valueToSet, visibility, requester, passphrase);
					valueToSet = decryptable;
				}
				targetMember.SetValue(targetObject, valueToSet);
			}
			foreach (var val in values)
			{
				var targetMember = targetFields.FirstOrDefault(x => x.Name == val.Key);
				if (targetMember == null)
				{
					continue;
				}
				if (targetMember.GetCustomAttribute<NoPatchAttribute>() != null)
				{
					throw new Exception($"Cannot patch field {targetMember.Name}");
				}
				var viewAttr = targetMember.GetCustomAttribute<ViewAttribute>();
				if (viewAttr == null || viewAttr.EditPermission > visibility)
				{
					throw new Exception($"Cannot patch field {targetMember.Name}: Insufficient privileges");
				}
				object objToPatch = targetObject;
				var value = targetMember.GetValue(targetObject);
				var fieldValue = value;
				object valueToSet = val.Value;
				if (typeof(IDecryptable).IsAssignableFrom(targetMember.FieldType) &&
					(value == null || !(value as IDecryptable).TryGetValue(requester, passphrase, out value)))
				{
					// Auth is incorrect
					continue;
				}
				try
				{
					var subValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(valueToSet.ToString());
					valueToSet = value.PatchObject(subValues, visibility, requester, passphrase);
				}
				catch
				{
				}
				if (typeof(IDecryptable).IsAssignableFrom(targetMember.FieldType))
				{
					var decryptable = fieldValue as IDecryptable;
					decryptable.SetValue(valueToSet, visibility, requester, passphrase);
					valueToSet = decryptable;
				}
				if (targetObject.GetType().IsValueType)
				{
					targetMember.SetValueDirect(__makeref(targetObject), valueToSet);
				}
				else
				{
					targetMember.SetValue(targetObject, valueToSet);
				}
			}
			return targetObject;
		}
	}
}
