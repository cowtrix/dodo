using System;
using System.Linq;

namespace CSVTools
{
	public abstract class SchemaObject
	{
		public SchemaObject(CharSeperatedSpreadsheetReader.Row row)
		{
			var allFields = GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			foreach (var field in allFields)
			{
				var attr = field.GetCustomAttributes(typeof(AliasAttribute), true).SingleOrDefault() as AliasAttribute;
				var columnKey = field.Name;
				if (attr != null)
				{
					columnKey = attr.Alias;
				}
				
				var fieldType = field.FieldType;
				var typeAlias = fieldType.GetCustomAttributes(typeof(AliasAttribute), true).SingleOrDefault() as AliasAttribute;
				columnKey = typeAlias != null ? typeAlias.Alias : columnKey;

				var strValue = row.Data[columnKey];

				if (fieldType == typeof(string))
					field.SetValue(this, strValue);
				else if (fieldType == typeof(byte))
				{
					field.SetValue(this, byte.Parse(strValue));
				}
				else if (typeof(Enum).IsAssignableFrom(fieldType))
				{
					foreach(Enum val in Enum.GetValues(fieldType))
					{
						var enumValueAlias = val.GetAttributeOfType<AliasAttribute>();
						if(enumValueAlias?.Alias == strValue)
						{
							strValue = val.ToString();
						}
					}
					field.SetValue(this, Enum.Parse(fieldType, strValue));
				}
			}
		}
	}

	
}
