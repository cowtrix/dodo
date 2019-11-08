using System;

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
		public static string GetName(this Enum enumVal)
		{
			var type = enumVal.GetType();
			var memInfo = type.GetMember(enumVal.ToString());
			var attributes = memInfo[0].GetCustomAttributes(typeof(NameAttribute), false);
			var nameAttr = (attributes.Length > 0) ? (NameAttribute)attributes[0] : null;
			if (nameAttr == null)
			{
				return enumVal.ToString();
			}
			return nameAttr.Name;
		}
	}
}
