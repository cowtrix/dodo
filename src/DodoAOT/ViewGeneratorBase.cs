using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Common;
using Resources.Location;

namespace DodoAOT
{
	public class ViewGeneratorBase
	{
		private delegate IEnumerable<string> CustomTypeCallback(Type baseType, MemberInfo member, int indentLevel);
		private static Dictionary<Type, CustomTypeCallback> m_customTypeCallback = new Dictionary<Type, CustomTypeCallback>()
		{
			//{ typeof(GeoLocation), GetLocationEditor }
		};

		private static IEnumerable<string> GetLocationEditor(Type baseType, MemberInfo member, int indentLevel)
		{
			throw new NotImplementedException();
		}

		protected static string Indent(int indent)
		{
			return new string('\t', indent);
		}

		protected static IEnumerable<string> BuildClass(Type targetType, int indentLevel, string prefix)
		{
			foreach (var member in targetType.GetPropertiesAndFields(p => p.CanRead && p.CanWrite, f => true)
				.OrderBy(m => m.DeclaringType?.InheritanceHierarchy().Count()))
			{
				var viewAttr = member.GetCustomAttribute<ViewAttribute>();
				if (viewAttr == null)
				{
					continue;
				}
				var memberType = member.GetMemberType();
				var typeName = memberType.GetRealTypeName(true);
				var memberName = member.Name;

				if (m_customTypeCallback.TryGetValue(memberType, out var callback))
				{
					foreach (var line in callback(targetType, member, indentLevel))
					{
						yield return line;
					}
					continue;
				}

				if (!memberType.IsPrimitive && !memberType.Namespace.StartsWith(nameof(System)))
				{
					yield return Indent(indentLevel) + $"<div class=\"card\">";
					yield return Indent(indentLevel + 1) + $"<h5 class=\"card-title\">{member.GetName()}</h5>";
					yield return Indent(indentLevel + 2) + $"<div class=\"card-body\">";
					foreach (var line in BuildClass(memberType, indentLevel + 3, $"{prefix}{memberName}."))
					{
						yield return line;
					}
					yield return Indent(indentLevel + 2) + $"</div>";
					yield return Indent(indentLevel) + $"</div>";
				}
				else
				{
					string inputExtras = "";
					if (member.GetCustomAttribute<PasswordAttribute>() != null)
					{
						inputExtras += "type=\"password\"";
					}
					yield return Indent(indentLevel) + $"<div class=\"form-group-row\">";
					yield return Indent(indentLevel + 1) + $"<label asp-for=\"{prefix}{memberName}\" class=\"control-label\"></label>";
					yield return Indent(indentLevel + 1) + $"<input {inputExtras} asp-for=\"{prefix}{memberName}\" class=\"form-control\" />";
					yield return Indent(indentLevel + 1) + $"<span asp-validation-for=\"{prefix}{memberName}\" class=\"text-danger\"></span>";
					yield return Indent(indentLevel) + $"</div>";
				}
			}
		}
	}
}
