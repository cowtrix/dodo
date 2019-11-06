using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	/// <summary>
	/// This signifies that a property or field cannot be updated via a PATCH REST command and must be updated
	/// through some internal method
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class NoPatchAttribute : Attribute { }

	public static class ReflectionExtensions
	{
		public static IEnumerable<Type> GetChildClasses<T>() where T:class
		{
			return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetTypes()).ConcatenateCollection()
				.Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
		}

		public static T GetCustomAttribute<T>(MemberInfo info) where T:Attribute
		{
			return info.GetCustomAttribute(typeof(T)) as T;
		}

		public static void PatchObject<T1, T2>(this T2 targetObject, T1 anonymousObj) where T1 : class where T2 : class
		{
			var anonymousProperties = typeof(T1).GetProperties().Where(x => x.CanRead);
			var targetProperties = typeof(T2).GetProperties().Where(x => x.CanRead);
			foreach (var anonProp in anonymousProperties)
			{
				var targetProp = targetProperties.Single(x => x.Name == anonProp.Name);
				if(targetProp.GetCustomAttribute<NoPatchAttribute>() != null)
				{
					throw new Exception($"Cannot patch property {targetProp.Name}");
				}
				targetProp.SetValue(targetObject, anonProp.GetValue(anonymousObj));
			}

			var anonymousFields = typeof(T1).GetFields();
			var targetFields = typeof(T2).GetFields();
			foreach (var anonField in anonymousProperties)
			{
				var targetField = targetFields.Single(x => x.Name == anonField.Name);
				if (targetField.GetCustomAttribute<NoPatchAttribute>() != null)
				{
					throw new Exception($"Cannot patch field {targetField.Name}");
				}
				targetField.SetValue(targetObject, anonField.GetValue(anonymousObj));
			}
		}

		/// <summary>
		/// Given a string/object dictionary, where the string is the name of a field or property and the
		/// object is the value to be set, patch the values of a target object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="targetObject"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static T PatchObject<T>(this T targetObject, Dictionary<string, object> values) where T : class
		{
			var targetProperties = targetObject.GetType().GetProperties();
			foreach (var val in values)
			{
				var targetProp = targetProperties.FirstOrDefault(x => x.Name == val.Key);
				if(targetProp == null)
				{
					continue;
				}
				if (targetProp.GetCustomAttribute<NoPatchAttribute>() != null)
				{
					throw new Exception($"Cannot patch property {targetProp.Name}");
				}
				try
				{
					var subValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(val.Value.ToString());
					var subTargetObject = targetProp.GetValue(targetObject);
					subTargetObject = subTargetObject.PatchObject(subValues);
					targetProp.SetValue(targetObject, subTargetObject);
				}
				catch
				{
					targetProp.SetValue(targetObject, val.Value);
				}
			}
			var targetFields = targetObject.GetType().GetFields();
			foreach (var val in values)
			{
				var targetField = targetFields.FirstOrDefault(x => x.Name == val.Key);
				if (targetField == null)
				{
					continue;
				}
				if (targetField.GetCustomAttribute<NoPatchAttribute>() != null)
				{
					throw new Exception($"Cannot patch field {targetField.Name}");
				}
				try
				{
					var subValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(val.Value.ToString());
					var subTargetObject = targetField.GetValue(targetObject);
					subTargetObject = subTargetObject.PatchObject(subValues);
					targetField.SetValue(targetObject, subTargetObject);
				}
				catch
				{
					targetField.SetValue(targetObject, val.Value);
				}
			}
			return targetObject;
		}
	}
}
