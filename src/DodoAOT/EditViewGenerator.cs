using Resources;
using Resources.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Common;
using System.Text;
using System.IO;

namespace DodoAOT
{
	public static class EditViewGenerator
	{
		private static IEnumerable<string> BuildClass(Type targetType, int indentLevel, string prefix)
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

				if (!memberType.IsPrimitive && !memberType.Namespace.StartsWith(nameof(System)))
				{
					yield return new string('\t', indentLevel) + $"<div class=\"card\">";
					yield return new string('\t', indentLevel + 1) + $"<h5 class=\"card-title\">{member.GetName()}</h5>";
					yield return new string('\t', indentLevel + 2) + $"<div class=\"card-body\">";
					foreach (var line in BuildClass(memberType, indentLevel + 3, $"{prefix}{memberName}."))
					{
						yield return line;
					}
					yield return new string('\t', indentLevel + 2) + $"</div>";
					yield return new string('\t', indentLevel) + $"</div>";
				}
				else
				{
					yield return new string('\t', indentLevel) + $"<div class=\"form-group\">";
					yield return new string('\t', indentLevel + 1) + $"<label asp-for=\"{prefix}{memberName}\" class=\"control-label\"></label>";
					yield return new string('\t', indentLevel + 1) + $"<input asp-for=\"{prefix}{memberName}\" class=\"form-control\" {(viewAttr.EditPermission <= EPermissionLevel.ADMIN ? "" : $"readonly")}/>";
					yield return new string('\t', indentLevel + 1) + $"<span asp-validation-for=\"{prefix}{memberName}\" class=\"text-danger\"></span>";
					yield return new string('\t', indentLevel) + $"</div>";
				}
			}
		}

		public static string Generate(Type resourceType)
		{
			var template = File.ReadAllText("Edit.template");
			template = template.Replace("{TYPE}", resourceType.Name);
			template = template.Replace("{NAME}", resourceType.GetName());
			var view = new StringBuilder();
			foreach (var line in BuildClass(resourceType, 4, ""))
			{
				view.AppendLine(line);
			}
			var viewBody = view.ToString();

			template = template.Replace("{BODY}", viewBody);
			return template;
		}
	}
}
