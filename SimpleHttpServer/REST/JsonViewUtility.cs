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

		public ViewAttribute(EPermissionLevel viewPermission, EPermissionLevel editPermission = EPermissionLevel.ADMIN)
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
			typeof(GeoLocation),
			typeof(IResourceReference)
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
				vals.Add("PERMISSION", visibility.GetName());
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
					|| m_explicitValueTypes.Any(t => prop.PropertyType.IsAssignableFrom(t)))
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
					|| m_explicitValueTypes.Any(t => field.FieldType.IsAssignableFrom(t)))
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
					var val = field.GetValue(obj);
					object valueToStore;
					if (val is IEnumerable)
					{
						var list = new List<object>();
						foreach(var innerVal in (val as IEnumerable))
						{
							list.Add(innerVal.GenerateJsonView(visibility, requester, passPhrase));
						}
						valueToStore = list;
					}
					else
					{
						valueToStore = val.GenerateJsonView(visibility, requester, passPhrase);
					}
					vals.Add(field.Name, valueToStore);
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
			// Firstly, if we hit a primitive type or a specially included type, we just convert the whole thing
			// to an object of that type with Json
			if ((targetType.IsValueType && targetType.IsPrimitive)
					|| m_explicitValueTypes.Any(t => t.IsAssignableFrom(targetType)))
			{
				var val = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(values), targetType);
				if(val is IVerifiable)
				{
					(val as IVerifiable).CheckValue();
				}
				return (T)val;
			}
			// If the object is decryptable, we handle this transparently and redirect the data within
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

			// Get fields and properties
			var members = new List<MemberInfo>(targetObject.GetType().GetProperties());
			members.AddRange(targetObject.GetType().GetFields());

			var validFields = values.Select(kvp => members.FirstOrDefault(x => x.Name == kvp.Key))
				.Where(member => member != null);
			if(validFields.Count() != values.Count)
			{
				throw new Exception("Invalid field names");
			}
			if(validFields.Where(member => member.GetCustomAttribute<ViewAttribute>()?.EditPermission <= visibility).Count() != values.Count)
			{
				throw new Exception("Insufficient privileges");
			}

			// Define common actions to handle properties and fields
			Action<MemberInfo, object, object> SetValue = (member, target, val) =>
			{
				if (member is PropertyInfo)
					(member as PropertyInfo).SetValue(target, val);
				else if (member is FieldInfo)
				{
					if (target.GetType().IsValueType)
					{
						(member as FieldInfo).SetValueDirect(__makeref(target), val);
					}
					else
					{
						(member as FieldInfo).SetValue(target, val);
					}
				}
			};
			Func<MemberInfo, object, object> GetValue = (member, target) =>
			{
				if (member is PropertyInfo)
					return (member as PropertyInfo).GetValue(target);
				else if (member is FieldInfo)
					return (member as FieldInfo).GetValue(target);
				throw new Exception("Unsupported MemberInfo type" + member.GetType());
			};
			Func<MemberInfo, Type> GetMemberType = (member) =>
			{
				if (member is PropertyInfo)
					return (member as PropertyInfo).PropertyType;
				else if (member is FieldInfo)
					return (member as FieldInfo).FieldType;
				throw new Exception("Unsupported MemberInfo type" + member.GetType());
			};

			// Go through the dictionary and set the values
			foreach (var val in values)
			{
				var targetMember = validFields.FirstOrDefault(x => x.Name == val.Key);
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
				var value = GetValue(targetMember, targetObject);
				var fieldValue = value;
				var valueToSet = val.Value;
				if (typeof(IDecryptable).IsAssignableFrom(GetMemberType(targetMember)) &&
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
				if (typeof(IDecryptable).IsAssignableFrom(GetMemberType(targetMember)))
				{
					var decryptable = fieldValue as IDecryptable;
					decryptable.SetValue(valueToSet, visibility, requester, passphrase);
					valueToSet = decryptable;
				}
				var checkAttr = targetMember.GetCustomAttribute<VerifyMemberBase>();
				if(checkAttr != null && !checkAttr.Verify(valueToSet, out var error))
				{
					throw new Exception(error);
				}
				SetValue(targetMember, targetObject, valueToSet);
			}
			return targetObject;
		}
	}
}
