using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DodoTest.Framework
{
	public static class CompareHelper
	{
		public static bool CompareObjects(object inputObjectA, object inputObjectB, string[] ignorePropertiesList)
		{
			bool areObjectsEqual = true;
			//check if both objects are not null before starting comparing children
			if (inputObjectA != null && inputObjectB != null)
			{
				//create variables to store object values
				object value1, value2;

				PropertyInfo[] properties = inputObjectA.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

				//get all public properties of the object using reflection
				foreach (PropertyInfo propertyInfo in properties)
				{
					//if it is not a readable property or if it is a ignorable property
					//ingore it and move on
					if (!propertyInfo.CanRead || (ignorePropertiesList != null && ignorePropertiesList.Contains(propertyInfo.Name)))
						continue;

					//get the property values of both the objects
					value1 = propertyInfo.GetValue(inputObjectA, null);
					value2 = propertyInfo.GetValue(inputObjectB, null);

					// if the objects are primitive types such as (integer, string etc)
					//that implement IComparable, we can just directly try and compare the value
					if (IsAssignableFrom(propertyInfo.PropertyType) || IsPrimitiveType(propertyInfo.PropertyType) || IsValueType(propertyInfo.PropertyType))
					{
						//compare the values
						if (!CompareValues(value1, value2))
						{
							Console.WriteLine("Property Name {0}", propertyInfo.Name);
							areObjectsEqual = false;
						}
					}
					//if the property is a collection (or something that implements IEnumerable)
					//we have to iterate through all items and compare values
					else if (IsEnumerableType(propertyInfo.PropertyType))
					{
						Console.WriteLine("Property Name {0}", propertyInfo.Name);
						CompareEnumerations(value1, value2, ignorePropertiesList);
					}
					//if it is a class object, call the same function recursively again
					else if (propertyInfo.PropertyType.IsClass)
					{
						if (!CompareObjects(propertyInfo.GetValue(inputObjectA, null), propertyInfo.GetValue(inputObjectB, null), ignorePropertiesList))
						{
							areObjectsEqual = false;
						}
					}
					else
					{
						areObjectsEqual = false;
					}
				}
			}
			else
				areObjectsEqual = false;

			return areObjectsEqual;
		}

		//true if c and the current Type represent the same type, or if the current Type is in the inheritance
		//hierarchy of c, or if the current Type is an interface that c implements,
		//or if c is a generic type parameter and the current Type represents one of the constraints of c. false if none of these conditions are true, or if c is null.
		private static bool IsAssignableFrom(Type type)
		{
			return typeof(IComparable).IsAssignableFrom(type);
		}

		private static bool IsPrimitiveType(Type type)
		{
			return type.IsPrimitive;
		}

		private static bool IsValueType(Type type)
		{
			return type.IsValueType;
		}

		private static bool IsEnumerableType(Type type)
		{
			return (typeof(IEnumerable).IsAssignableFrom(type));
		}

		/// <summary>
		/// Compares two values and returns if they are the same.
		/// </summary>
		private static bool CompareValues(object value1, object value2)
		{
			bool areValuesEqual = true;
			IComparable selfValueComparer = value1 as IComparable;

			// one of the values is null
			if (value1 == null && value2 != null || value1 != null && value2 == null)
				areValuesEqual = false;
			else if (selfValueComparer != null && selfValueComparer.CompareTo(value2) != 0)
				areValuesEqual = false;
			else if (!object.Equals(value1, value2))
				areValuesEqual = false;

			return areValuesEqual;
		}

		private static bool CompareEnumerations(object value1, object value2, string[] ignorePropertiesList)
		{
			// if one of the values is null, no need to proceed return false;
			if (value1 == null && value2 != null || value1 != null && value2 == null)
				return false;
			else if (value1 != null && value2 != null)
			{
				IEnumerable<object> enumValue1, enumValue2;
				enumValue1 = ((IEnumerable)value1).Cast<object>();
				enumValue2 = ((IEnumerable)value2).Cast<object>();

				// if the items count are different return false
				if (enumValue1.Count() != enumValue2.Count())
					return false;
				// if the count is same, compare individual item
				else
				{
					object enumValue1Item, enumValue2Item;
					Type enumValue1ItemType;
					for (int itemIndex = 0; itemIndex < enumValue1.Count(); itemIndex++)
					{
						enumValue1Item = enumValue1.ElementAt(itemIndex);
						enumValue2Item = enumValue2.ElementAt(itemIndex);
						enumValue1ItemType = enumValue1Item.GetType();
						if (IsAssignableFrom(enumValue1ItemType) || IsPrimitiveType(enumValue1ItemType) || IsValueType(enumValue1ItemType))
						{
							if (!CompareValues(enumValue1Item, enumValue2Item))
								return false;
						}
						else if (!CompareObjects(enumValue1Item, enumValue2Item, ignorePropertiesList))
							return false;
					}
				}
			}
			return true;
		}
	}

}
