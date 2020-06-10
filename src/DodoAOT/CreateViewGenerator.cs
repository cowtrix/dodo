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
	public static class CreateViewGenerator
	{
		private static IEnumerable<string> BuildClass(Type targetType, int indentLevel, string prefix)
		{
			var allMembers = new List<MemberInfo>(targetType.GetProperties().Where(p => p.CanRead));
			allMembers.AddRange(targetType.GetFields());
			foreach (var member in allMembers)
			{
				var memberType = member.GetMemberType();
				var typeName = memberType.GetRealTypeName(true);
				var memberName = member.Name;

				if (!memberType.IsPrimitive && !memberType.Namespace.StartsWith(nameof(System)))
				{
					yield return new string('\t', indentLevel) + $"<div class=\"object-group\">";
					foreach(var line in BuildClass(memberType, indentLevel + 1, $"{prefix}{memberName}."))
					{
						yield return line;
					}
					yield return new string('\t', indentLevel) + $"</div>";
				}
				else
				{
					yield return new string('\t', indentLevel) + $"<div class=\"form-group\">";
					yield return new string('\t', indentLevel + 1) + $"<label asp-for=\"{prefix}{memberName}\" class=\"control-label\"></label>";
					yield return new string('\t', indentLevel + 1) + $"<input asp-for=\"{prefix}{memberName}\" class=\"form-control\" />";
					yield return new string('\t', indentLevel + 1) + $"<span asp-validation-for=\"{prefix}{memberName}\" class=\"text-danger\"></span>";
					yield return new string('\t', indentLevel) + $"</div>";
				}
			}
		}

		public static string GenerateCreateView(Type resourceType)
		{
			var schemaType = ResourceUtility.GetFactory(resourceType).SchemaType;
			var template = File.ReadAllText("Create.template");
			template = template.Replace("{SCHEMA_TYPE}", schemaType.FullName);

			var view = new StringBuilder();
			foreach (var line in BuildClass(schemaType, 2, ""))
			{
				view.AppendLine(line);
			}
			var viewBody = view.ToString();

			template = template.Replace("{BODY}", viewBody);
			return template;
		}
	}
}
