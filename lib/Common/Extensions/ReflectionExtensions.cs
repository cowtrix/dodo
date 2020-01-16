using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Extensions
{
	public static class ReflectionExtensions
	{
		public static IEnumerable<Type> GetChildClasses<T>() where T:class
		{
			return AppDomain.CurrentDomain.GetAssemblies()
				.Where(ass => !ass.GetName().FullName.Contains("Microsoft"))  // TODO https://developercommunity.visualstudio.com/content/problem/738856/could-not-load-file-or-assembly-microsoftintellitr.html
				.Select(assembly => assembly.GetTypes()).ConcatenateCollection()
				.Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);
		}

		public static T GetCustomAttribute<T>(MemberInfo info) where T:Attribute
		{
			return info.GetCustomAttribute(typeof(T)) as T;
		}

		public static Type GetMemberType(this MemberInfo member)
		{
			if (member is PropertyInfo)
				return (member as PropertyInfo).PropertyType;
			else if (member is FieldInfo)
				return (member as FieldInfo).FieldType;
			throw new Exception("Unsupported MemberInfo type" + member.GetType());
		}

		public static void SetValue(this MemberInfo member, object target, object val)
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
		}

		public static object GetValue(this MemberInfo member, object target)
		{
			if (member is PropertyInfo)
				return (member as PropertyInfo).GetValue(target);
			else if (member is FieldInfo)
				return (member as FieldInfo).GetValue(target);
			return null;
		}
	}
}
