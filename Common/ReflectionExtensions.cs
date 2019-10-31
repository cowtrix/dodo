using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public static class ReflectionExtensions
	{
		public static IEnumerable<Type> GetChildClasses<T>() where T:class
		{
			return AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetTypes()).ConcatenateCollection()
				.Where(t => typeof(T).IsAssignableFrom(t) && !t.IsAbstract);
		}

		public static T GetCustomAttribute<T>(MemberInfo info) where T:Attribute
		{
			return info.GetCustomAttribute(typeof(T)) as T;
		}
	}
}
