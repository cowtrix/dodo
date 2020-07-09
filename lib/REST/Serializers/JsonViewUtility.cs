using Common;
using Common.Extensions;
using Resources.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Resources.Location;
using System.Collections.Concurrent;

namespace Resources
{
	[AttributeUsage(AttributeTargets.Struct)]
	public class ViewClassAttribute : Attribute
	{
	}

	/// <summary>
	/// This class will performs Resource specific JSON parsing tasks.
	/// </summary>
	public static class JsonViewUtility
	{
		private static ConcurrentDictionary<(Guid, uint), Dictionary<string, object>> m_cache = new ConcurrentDictionary<(Guid, uint), Dictionary<string, object>>();

		private static readonly HashSet<Type> m_explicitValueTypes = new HashSet<Type>()
		{
			typeof(string),
			typeof(Guid),
			typeof(Enum),
			typeof(DateTime),
			typeof(GeoLocation),
			typeof(IResourceReference)
		};

		public static T CopyByValue<T>(this object obj, object requester, Passphrase passphrase) where T:class
		{
			var newT = Activator.CreateInstance<T>() as T;
			var sourceType = obj.GetType();
			if(obj is IDecryptable decryptable)
			{
				if(!decryptable.TryGetValue(requester, passphrase, out obj))
				{
					// User wasn't authorised
					SecurityWatcher.RegisterEvent(requester as IRESTResource, "Bad authorization");
					return null;
				}
				sourceType = obj.GetType();
			}
			foreach (var prop in typeof(T).GetPropertiesAndFields(p => p.CanWrite, f => true))
			{
				var name = prop.Name;
				var sourcePropCandidates = sourceType.GetPropertiesAndFields(p => p.Name == name, f => f.Name == name);
				if(!sourcePropCandidates.Any())
				{
					throw new Exception($"Failed to find property {name} in {sourceType}");
				}
				else if (sourcePropCandidates.Count() > 1)
				{
					throw new Exception($"Ambigious property match for {name} in {sourceType}");
				}
				var sourceProp = sourcePropCandidates.Single();
				object value = sourceProp.GetValue(obj);
				if(value == null)
				{
					continue;
				}
				if (value.GetType() != prop.GetMemberType())
				{
					value = typeof(JsonViewUtility).GetMethod(nameof(CopyByValue)).MakeGenericMethod(prop.GetMemberType())
						.Invoke(null, new [] { value, requester, passphrase });
				}
				prop.SetValue(newT, value);
			}
			return newT;
		}

		public static Dictionary<string, object> GetJsonSchema(Type targetType)
		{
			var vals = new Dictionary<string, object>();

			var allMembers = new List<MemberInfo>(targetType.GetProperties().Where(p => p.CanRead));
			allMembers.AddRange(targetType.GetFields());
			foreach (var member in allMembers)
			{
				var attr = member.GetCustomAttribute<ViewAttribute>();
				if (attr == null || attr.ViewPermission > EPermissionLevel.ADMIN)
				{
					continue;
				}
				var memberType = member.GetMemberType();
				if (typeof(IDecryptable).IsAssignableFrom(memberType))
				{
					memberType = memberType.InheritanceHierarchy().First(t => t.IsGenericType).GetGenericArguments().First();
				}

				var memberName = member.Name.ToCamelCase();

				if (!ShouldSerializeDirectly(memberType))
				{
					var subObjSchema = GetJsonSchema(memberType);
					if (subObjSchema.Any())
					{
						vals.Add(memberName,
						new
						{
							type = memberType.GetRealTypeName(),
							view = attr.ViewPermission.ToString().ToLowerInvariant(),
							edit = attr.EditPermission.ToString().ToLowerInvariant(),
							schema = subObjSchema,
						});
						continue;
					}
				}

				vals.Add(memberName,
				new
				{
					type = memberType.GetRealTypeName(),
					view = attr.ViewPermission.ToString().ToLowerInvariant(),
					edit = attr.EditPermission.ToString().ToLowerInvariant(),
				});
			}
			return vals;
		}

		/// <summary>
		/// This will generate a JSON object that represents viewable properties of this object.
		/// An object is marked as viewable with the ViewAttribute. Fields and properties
		/// are filtered by the requester's EPermissionLevel
		/// </summary>
		/// <returns>A string/object dictionary where the string value is the name of a field and the object is its value</returns>
		public static Dictionary<string, object> GenerateJsonView(this object obj, EPermissionLevel visibility,
			object requester, Passphrase passphrase)
		{
			lock (obj)
			{
				if (obj == null)
				{
					return null;
				}

				if (obj is IRESTResource resource)
				{
					// Try to hit the resource cache
					if (m_cache.TryGetValue((resource.Guid, resource.Revision), out var cacheVal))
					{
						return cacheVal;
					}
				}

				// Handle composite/primitive object case
				var targetType = obj.GetType();
				if (ShouldSerializeDirectly(targetType))
				{
					throw new Exception($"Cannot generate JSON view for type {targetType}");
				}

				var vals = new Dictionary<string, object>();


				// Get fields and properties, filter to what we can view with our permission level
				var allMembers = new List<MemberInfo>(targetType.GetProperties().Where(p => p.CanRead));
				allMembers.AddRange(targetType.GetFields());
				var filteredMembers = allMembers.Where(m =>
				{
					var attr = m.GetCustomAttribute<ViewAttribute>();
					if (attr == null || attr.ViewPermission > visibility)
					{
						return false;
					}
					return true;
				});

				foreach (var member in filteredMembers)
				{
					var memberName = member.Name.ToCamelCase();
					var targetPropValue = member.GetValue(obj);
					var memberType = member.GetMemberType();
					var finalObj = GetObject(targetPropValue, memberType, requester, visibility, passphrase);
					if (finalObj != null)
					{
						vals.Add(memberName, finalObj);
					}
				}
				if (obj is IViewMetadataProvider view)
				{
					var metadata = new Dictionary<string, object>();
					view.AppendMetadata(metadata, visibility, requester, passphrase);
					vals.Add(Resource.METADATA, metadata);
				}

				if (obj is IRESTResource finalRsc)
				{
					m_cache[(finalRsc.Guid, finalRsc.Revision)] = vals;
					return vals;
				}
				return vals;
			}
		}

		/// <summary>
		/// Generate a JSON view of an IEnumerable.
		/// </summary>
		/// <returns></returns>
		public static List<Dictionary<string, object>> GenerateJsonViewEnumerable<T>(this IEnumerable<T> obj,
			EPermissionLevel visibility, object requester, Passphrase passphrase)
		{
			return obj.Select(x => x.GenerateJsonView(visibility, requester, passphrase)).ToList();
		}

		public static T PatchObject<T>(this T targetObject, Dictionary<string, object> values)
		{
			return PatchObject(targetObject, values, EPermissionLevel.OWNER, null, default);
		}

		public static T PatchObject<T>(this T targetObject, object value, EPermissionLevel permissionLevel,
			object requester, Passphrase passphrase)
		{
			return PatchObject(targetObject, ViewToPatch(value, permissionLevel), permissionLevel, requester, passphrase);
		}

		public static Dictionary<string, object> ViewToPatch(object obj, EPermissionLevel permissionLevel)
		{
			if(obj is Dictionary<string, object> dict)
			{
				return dict;
			}
			var patch = new Dictionary<string, object>();
			foreach(var member in obj.GetType().GetPropertiesAndFields(p => true, f => true))
			{
				var viewAttr = member.GetCustomAttribute<ViewAttribute>();
				if(viewAttr == null || permissionLevel < viewAttr.EditPermission)
				{
					continue;
				}
				var value = member.GetValue(obj);
				if (!ShouldSerializeDirectly(member.GetMemberType()))
				{
					patch[member.Name] = ViewToPatch(value, permissionLevel);
				}
				else
				{
					patch[member.Name] = value;
				}
			}
			return patch;
		}

		/// <summary>
		/// Given a string/object dictionary, where the string is the name of a field or property and the
		/// object is the value to be set, set the values of a target object to the given values.
		/// </summary>
		/// <typeparam name="T">The type of the object we are patching</typeparam>
		/// <param name="targetObject">The object to patch</param>
		/// <param name="values">The values to set</param>
		/// <param name="permissionLevel">The permission level of the requester</param>
		/// <param name="requester">The key for encrypting/decrypting objects</param>
		/// <param name="passphrase">The passphrase for encrypting/decrypting objects</param>
		/// <returns></returns>
		public static T PatchObject<T>(this T targetObject, Dictionary<string, object> values, EPermissionLevel permissionLevel,
			object requester, Passphrase passphrase)
		{
			if (permissionLevel == EPermissionLevel.SYSTEM)
			{
				throw new ArgumentException($"Patch permission level cannot be {nameof(EPermissionLevel.SYSTEM)}." +
					" Members with this permission level can only be altered from direct code calls.");
			}

			// If the object is decryptable, we handle this transparently and redirect the data within
			if (targetObject is IDecryptable decryptable)
			{
				if (!decryptable.TryGetValue(requester, passphrase, out var encryptedObject))
				{
					// User wasn't authorised
					SecurityWatcher.RegisterEvent(requester as IRESTResource, "Bad authorization");
				}
				encryptedObject.PatchObject(values, permissionLevel, requester, passphrase);
				decryptable.SetValue(encryptedObject, permissionLevel, requester, passphrase);
				return targetObject;
			}

			// Check for case insensitive duplicates
			foreach (var key in values.Keys)
			{
				if (values.Keys.Count(k => string.Equals(key, k, StringComparison.OrdinalIgnoreCase)) != 1)
				{
					throw new Exception($"Duplicate key {key} - field names are case insensitive and must be unique.");
				}
			}

			string error = null;
			var targetType = targetObject != null ? targetObject.GetType() : typeof(T);
			// Firstly, if we hit a primitive type or a specially included type, we just convert the whole thing
			// to an object of that type with Json
			if (ShouldSerializeDirectly(targetType))
			{
				var val = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(values, JsonExtensions.NetworkSettings), targetType, JsonExtensions.NetworkSettings);
				return (T)val;
			}

			if (targetType.IsValueType)
			{
				throw new Exception("Cannot patch immutable struct - you must pass the full object. " +
					"Add the [ViewClass] attribute to your struct.");
			}
			
			// Get fields and properties
			var members = new List<MemberInfo>(targetObject.GetType().GetProperties());
			members.AddRange(targetObject.GetType().GetFields());

			// Validate that we are able to complete this operation
			// Really, the client shouldn't ever do a bad request
			// So any problems here may indicate an attack
			var validFields = values.Select(kvp => members.FirstOrDefault(x => x.Name.Equals(kvp.Key, StringComparison.OrdinalIgnoreCase)))
				.Where(member => member != null);
			if (validFields.Count() != values.Count)
			{
				// Request submitted incorrect fields
				throw new Exception("Invalid field names");
			}
			if (validFields.Where(member => member.GetCustomAttribute<ViewAttribute>()?.EditPermission <= permissionLevel)
				.Count() != values.Count)
			{
				// The request is trying to modify fields it is not allowed to
				throw new Exception("Insufficient privileges");
			}
			if (!validFields.Where(m => m.GetMemberType() is IDecryptable).Cast<IDecryptable>().All(decryptable => decryptable.IsAuthorised(requester, passphrase)))
			{
				throw new Exception("Authorisation failed");
			}

			// Go through the dictionary and set the values
			foreach (var val in values)
			{
				var targetMember = validFields.FirstOrDefault(x => x.Name.Equals(val.Key, StringComparison.OrdinalIgnoreCase));
				if (targetMember == null)
				{
					continue;
				}
				var viewAttr = targetMember.GetCustomAttribute<ViewAttribute>();
				if (viewAttr == null || viewAttr.EditPermission > permissionLevel)
				{
					throw new Exception($"Cannot patch field {targetMember.Name}: Insufficient privileges");
				}
				object objToPatch = targetObject;
				var value = targetMember.GetValue(targetObject);
				var fieldValue = value;
				object valueToSet = val.Value;
				if (typeof(IDecryptable).IsAssignableFrom(targetMember.GetMemberType()) &&
					(value == null || !(value as IDecryptable).TryGetValue(requester, passphrase, out value)))
				{
					// Auth is incorrect, but that should have been picked up before
					throw new Exception("Unexpected authorization failure on " + targetMember.Name);
				}
				if (!string.IsNullOrEmpty(val.Value.ToString()))
				{
					try
					{
						var subValues = val.Value as Dictionary<string, object>;
						if (subValues == null)
						{
							subValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(val.Value.ToString(), JsonExtensions.NetworkSettings);
						}
						valueToSet = value.PatchObject(subValues, permissionLevel, requester, passphrase);
					}
					catch (MemberVerificationException)
					{
						throw;
					}
					catch
					{
					}
				}
				var memberType = targetMember.GetMemberType();
				if (ShouldSerializeDirectly(memberType) && memberType != valueToSet?.GetType())
				{
					var serialized = JsonConvert.SerializeObject(valueToSet, memberType, JsonExtensions.NetworkSettings);
					valueToSet = JsonConvert.DeserializeObject(serialized, memberType, JsonExtensions.NetworkSettings);
				}
				if (typeof(IDecryptable).IsAssignableFrom(targetMember.GetMemberType()))
				{
					decryptable = fieldValue as IDecryptable;
					decryptable.SetValue(valueToSet, permissionLevel, requester, passphrase);
					valueToSet = decryptable;
				}
				var verifyAttributes = targetMember.GetCustomAttributes().OfType<IVerifiableMember>();
				foreach(var verifyAttr in verifyAttributes)
				{
					if (verifyAttr != null && !verifyAttr.Verify(valueToSet, out error))
					{
						throw new MemberVerificationException(error);
					}
				}
				targetMember.SetValue(targetObject, valueToSet);
			}
			if (targetObject is IVerifiable verifiable && !verifiable.Verify(out error))
			{
				throw new MemberVerificationException(error);
			}
			return targetObject;
		}

		private static object GetObject(object targetPropValue, Type memberType, object requester, EPermissionLevel visibility, Passphrase passphrase)
		{
			if (memberType == null || targetPropValue == null)
			{
				return null;
			}
			if (ShouldSerializeDirectly(memberType))
			{
				// Simple case, directly serialize object if it doesn't need any members filtered out
				return targetPropValue;
			}
			else if (typeof(IDecryptable).IsAssignableFrom(memberType))
			{
				// Transparently handle encrypted objects
				var decryptable = targetPropValue as IDecryptable;
				if (!decryptable.TryGetValue(requester, passphrase, out var data))
				{
					return null;
				}
				return GetObject(data, data.GetType(), requester, visibility, passphrase);
			}
			else if (targetPropValue is IEnumerable)
			{
				// Build lists of enumerable data
				var list = new List<object>();
				foreach (var innerVal in (targetPropValue as IEnumerable))
				{
					if (ShouldSerializeDirectly(innerVal?.GetType()))
					{
						list.Add(innerVal);
					}
					else
					{
						list.Add(innerVal.GenerateJsonView(visibility, requester, passphrase));
					}
				}
				return list;
			}
			// Object is a composite type (e.g. a struct or class) and so we recursively serialize it
			return targetPropValue.GenerateJsonView(visibility, requester, passphrase);
		}

		public static bool ShouldSerializeDirectly(Type targetType)
		{
			var viewAttr = targetType.GetCustomAttribute<ViewClassAttribute>();
			if (viewAttr != null)
			{
				return true;
			}
			return (targetType.IsValueType && targetType.IsPrimitive)
					|| m_explicitValueTypes.Any(t => t.IsAssignableFrom(targetType));
		}
	}
}
