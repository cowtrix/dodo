﻿using Common;
using Common.Extensions;
using REST.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace REST
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public class ViewClassAttribute : Attribute { }

	/// <summary>
	/// This class will performs Resource specific JSON parsing tasks.
	/// </summary>
	public static class JsonViewUtility
	{
		private static readonly HashSet<Type> m_explicitValueTypes = new HashSet<Type>()
		{
			typeof(string),
			typeof(Guid),
			typeof(Enum),
			typeof(GeoLocation),
			typeof(IResourceReference),
			typeof(DateTime),
		};

		/// <summary>
		/// This will generate a JSON object that represents viewable properties of this object.
		/// An object is marked as viewable with the ViewAttribute. Fields and properties
		/// are filtered by the requester's EPermissionLevel
		/// </summary>
		/// <returns>A string/object dictionary where the string value is the name of a field and the object is its value</returns>
		public static Dictionary<string, object> GenerateJsonView(this object obj, EPermissionLevel visibility,
			object requester, Passphrase passphrase)
		{
			if(obj == null)
			{
				return null;
			}
			var vals = new Dictionary<string, object>();

			// Get fields and properties, filter to what we can view with our permission level
			var targetType = obj.GetType();

			// Handle composite/primitive object case
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
				var memberName = member.Name;
				var targetPropValue = member.GetValue(obj);
				var memberType = member.GetMemberType();
				var finalObj = GetObject(targetPropValue, memberType, requester, visibility, passphrase);
				if(finalObj != null)
				{
					vals.Add(memberName, finalObj);
				}
			}
			if(obj is Resource)
			{
				(obj as Resource).AppendAuxilaryData(vals, visibility, requester, passphrase);
			}
			return vals;
		}

		/// <summary>
		/// Generate a JSON view of an IEnumerable.
		/// </summary>
		/// <returns></returns>
		public static List<Dictionary<string, object>> GenerateJsonView<T>(this IEnumerable<T> obj,
			EPermissionLevel visibility, object requester, Passphrase passphrase)
		{
			return obj.Select(x => x.GenerateJsonView(visibility, requester, passphrase)).ToList();
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
			var targetType = targetObject != null ? targetObject.GetType() : typeof(T);
			// Firstly, if we hit a primitive type or a specially included type, we just convert the whole thing
			// to an object of that type with Json
			if (ShouldSerializeDirectly(targetType))
			{
				var val = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(values, JsonExtensions.DefaultSettings), targetType, JsonExtensions.DefaultSettings);
				if(val is IVerifiable)
				{
					(val as IVerifiable).Verify();
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
				encryptedObject.PatchObject(values, permissionLevel, requester, passphrase);
				if (encryptedObject is IVerifiable)
				{
					(encryptedObject as IVerifiable).Verify();
				}
				decryptable.SetValue(encryptedObject, permissionLevel, requester, passphrase);
				return targetObject;
			}

			// Get fields and properties
			var members = new List<MemberInfo>(targetObject.GetType().GetProperties());
			members.AddRange(targetObject.GetType().GetFields());

			// Validate that we are able to complete this operation
			var validFields = values.Select(kvp => members.FirstOrDefault(x => x.Name == kvp.Key))
				.Where(member => member != null);
			if(validFields.Count() != values.Count)
			{
				throw new Exception("Invalid field names");
			}
			if(validFields.Where(member => member.GetCustomAttribute<ViewAttribute>()?.EditPermission <= permissionLevel).Count() != values.Count)
			{
				throw new Exception("Insufficient privileges");
			}
			if(!validFields.Where(m => m.GetMemberType() is IDecryptable).Cast<IDecryptable>().All(decryptable => decryptable.IsAuthorised(requester, passphrase)))
			{
				throw new Exception("Authorisation failed");
			}

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
				if(viewAttr == null || viewAttr.EditPermission > permissionLevel)
				{
					throw new Exception($"Cannot patch field {targetMember.Name}: Insufficient privileges");
				}
				object objToPatch = targetObject;
				var value = targetMember.GetValue(targetObject);
				var fieldValue = value;
				var valueToSet = val.Value;
				if (typeof(IDecryptable).IsAssignableFrom(targetMember.GetMemberType()) &&
					(value == null || !(value as IDecryptable).TryGetValue(requester, passphrase, out value)))
				{
					// Auth is incorrect, but that should have been picked up before
					throw new Exception("Unexpected authorization failure on " + targetMember.Name);
				}
				try
				{
					var subValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(val.Value.ToString(), JsonExtensions.DefaultSettings);
					valueToSet = value.PatchObject(subValues, permissionLevel, requester, passphrase);
				}
				catch(MemberVerificationException)
				{
					throw;
				}
				catch
				{
				}
				if (typeof(IDecryptable).IsAssignableFrom(targetMember.GetMemberType()))
				{
					var decryptable = fieldValue as IDecryptable;
					decryptable.SetValue(valueToSet, permissionLevel, requester, passphrase);
					valueToSet = decryptable;
				}
				var checkAttr = targetMember.GetCustomAttribute<VerifyMemberBase>();
				if(checkAttr != null && !checkAttr.Verify(valueToSet, out var error))
				{
					throw new Exception(error);
				}
				var verifyAttr = targetMember.GetCustomAttribute<VerifyMemberBase>();
				if(verifyAttr != null && !verifyAttr.Verify(valueToSet, out var verificationError))
				{
					throw new MemberVerificationException(verificationError);
				}
				targetMember.SetValue(targetObject, valueToSet);
			}
			if (targetObject is IVerifiable)
			{
				(targetObject as IVerifiable).Verify();
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

		private static bool ShouldSerializeDirectly(Type targetType)
		{
			var viewAttr = targetType.GetCustomAttribute<ViewClassAttribute>();
			if(viewAttr != null)
			{
				return true;
			}
			return (targetType.IsValueType && targetType.IsPrimitive)
					|| m_explicitValueTypes.Any(t => t.IsAssignableFrom(targetType));
		}
	}
}