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
		public static string GetName(this object obj)
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
	}
}
