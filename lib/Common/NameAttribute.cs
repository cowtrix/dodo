using System;
using System.Reflection;

namespace Common
{
	public class NameAttribute : Attribute
	{
		public string Name { get; private set; }
		public NameAttribute(string name)
		{
			Name = name;
		}
	}

	public static class NameAttributeExtensions
	{
		public static string GetName(this Enum obj)
		{
			if(obj == null)
			{
				return null;
			}
			var type = obj.GetType();
			var memInfo = type.GetMember(obj.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(NameAttribute), false);
			var nameAttr = (attributes.Length > 0) ? (NameAttribute)attributes[0] : null;
			if (nameAttr == null)
			{
				return obj.ToString();
			}
			return nameAttr.Name;
		}

		public static string GetName(this Type type)
		{
			if (type == null)
			{
				return null;
			}
			var nameAttr = type.GetCustomAttribute<NameAttribute>();
			if (nameAttr == null)
			{
				return type.Name;
			}
			return nameAttr.Name;
		}

		public static string GetName(this MemberInfo member)
		{
			if (member == null)
			{
				return null;
			}
			var nameAttr = member.GetCustomAttribute<NameAttribute>();
			if (nameAttr == null)
			{
				return member.Name;
			}
			return nameAttr.Name;
		}

		public static string GetName(this object obj)
		{
			if (obj == null)
			{
				return null;
			}
			var type = obj.GetType();
			var nameAttr = type.GetCustomAttribute<NameAttribute>();
			if (nameAttr == null)
			{
				return obj.ToString();
			}
			return nameAttr.Name;
		}
	}
}
