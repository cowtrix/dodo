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
		public static IEnumerable<T> GetChildClasses<T>() where T:class
		{
			return from t in Assembly.GetExecutingAssembly().GetTypes()
				   where t.GetInterfaces().Contains(typeof(T))
							&& t.GetConstructor(Type.EmptyTypes) != null
				   select Activator.CreateInstance(t) as T;
		}

		public static T GetCustomAttribute<T>(MemberInfo info) where T:Attribute
		{
			return info.GetCustomAttribute(typeof(T)) as T;
		}
	}
}
