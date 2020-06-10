using Resources;
using Resources.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Common;
using System.Text;

namespace DodoAOT
{
	public static class ViewModelGenerator
	{
		private static string GetFooter()
		{
			return "\t}\r\n}";
		}

		private static string GetHeader(Type key)
		{
			return $"// This is generated code from the DodoAOT project. DO NOT MODIFY. This code was generated on {DateTime.Now}\r\n" +
				"using System.ComponentModel;\r\n" +
				$"using {key.Namespace};\r\n" +
				"using Resources;\r\n" +
				"namespace Dodo.ViewModels\r\n{\r\n" +
				$"\tpublic class {key.Name}ViewModel\r\n\t{{\r\n";
		}

		private static IEnumerable<string> BuildClass(Type targetType, int indentLevel)
		{
			var allMembers = new List<MemberInfo>(targetType.GetProperties().Where(p => p.CanRead));
			allMembers.AddRange(targetType.GetFields());
			foreach (var member in allMembers)
			{
				var attr = member.GetCustomAttribute<ViewAttribute>();
				if (attr == null || attr.ViewPermission > EPermissionLevel.ADMIN)
				{
					continue;
				}
				var memberType = member.GetMemberType();
				if (typeof(IDecryptable).IsAssignableFrom(memberType))
				{
					memberType = memberType.InheritanceHierarchy().First(t => t.IsGenericType).GetGenericArguments().First();
				}
				var typeName = memberType.GetRealTypeName(true);
				var memberName = member.Name;
				if (!JsonViewUtility.ShouldSerializeDirectly(memberType) && !memberType.Namespace.StartsWith(nameof(System)))
				{
					typeName = $"{memberType.GetRealTypeName(false)}ViewModel";
					yield return new string('\t', indentLevel) + $"public class {typeName}";
					yield return new string('\t', indentLevel) + "{";
					foreach (var subStr in BuildClass(memberType, indentLevel + 1))
					{
						yield return subStr;
					}
					yield return new string('\t', indentLevel) + "}";
				}
				yield return new string('\t', indentLevel) + $"[DisplayName(\"{member.GetName()}\")]";
				yield return new string('\t', indentLevel) + $"[View({nameof(EPermissionLevel)}.{attr.ViewPermission}, {nameof(EPermissionLevel)}.{attr.EditPermission})]";
				yield return new string('\t', indentLevel) + $"public {typeName} {memberName} {{ get; set; }}";
			}
		}

		public static string Generate(Type type)
		{
			var sb = new StringBuilder();
			sb.Append(GetHeader(type));
			foreach (var line in BuildClass(type, 2))
			{
				sb.AppendLine(line);
			}
			sb.Append(GetFooter());
			return sb.ToString();
		}
	}
}
