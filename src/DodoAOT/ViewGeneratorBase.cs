using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Extensions;
using Common;
using Resources.Location;
using Resources.Security;
using Dodo.Users;
using System.IO;
using System.Text;
using Dodo.Users.Tokens;
using Dodo;

namespace DodoAOT
{
	public class ViewGeneratorBase
	{
		private delegate IEnumerable<string> CustomTypeCallback(string prefix, MemberInfo member, int indentLevel);
		private static Dictionary<Type, CustomTypeCallback> m_customTypeCallback = new Dictionary<Type, CustomTypeCallback>()
		{
			{ typeof(AdministrationData), Null },
			{ typeof(GeoLocation), GetLocationEditor },
			{ typeof(IResourceReference), RefView },
		};

		private static IEnumerable<string> Null(string prefix, MemberInfo member, int indentLevel)
		{
			yield return "";
		}

		protected static string GetAdminEditor(Type resourceType)
		{
			if (!typeof(IAdministratedResource).IsAssignableFrom(resourceType))
			{
				return "";
			}
			var field = resourceType.GetProperty("AdministratorData");
			var template = File.ReadAllText("Admin.template.cshtml");
			template = template.Replace("{NAME}", field.GetName());
			template = template.Replace("{MEMBER}", field.GetName());
			return template;
		}

		protected static string GetNotificationEditor(Type resourceType)
		{
			if (!typeof(INotificationResource).IsAssignableFrom(resourceType))
			{
				return "";
			}
			var template = File.ReadAllText("Notifications.template");
			var sb = new StringBuilder();
			foreach (var l in BuildNotificationView(resourceType)) sb.AppendLine(l);
			template = template.Replace("{BODY}", sb.ToString());
			return template;
		}

		private static IEnumerable<string> BuildNotificationView(Type resourceType)
		{
			var indent = 3;
			yield return Indent(indent) + "<div class=\"card\" style=\"width:100%;\">";
			yield return Indent(indent + 1) + "<div class=\"card-body\">";
			yield return Indent(indent + 2) + $"@notification.Message";
			yield return Indent(indent + 1) + "</div>";
			yield return Indent(indent) + "</div>";
		}

		protected static IEnumerable<string> BuildScripts()
		{
			string scriptPath = Path.GetFullPath("Scripts");
			if (!Directory.Exists(scriptPath))
			{
				throw new DirectoryNotFoundException(scriptPath);
			}
			var scripts = Directory.GetFiles(scriptPath);
			foreach (var f in scripts)
			{
				yield return File.ReadAllText(f);
			}
		}

		private static IEnumerable<string> RefView(string prefix, MemberInfo member, int indentLevel)
		{
			var nameStr = $"@Model.{prefix}{member?.Name}.{nameof(IResourceReference.Name)}";
			var urlStr = $"@Model.{prefix}{member?.Name}.{nameof(IResourceReference.Type)}/@Model.{prefix}{member?.Name}.{nameof(IResourceReference.Slug)}";
			if (member.GetMemberType() == typeof(User))
			{
				throw new Exception("Can't link to user profiles");
			}
			yield return Indent(indentLevel) + $"<div class=\"form-group\">";
			yield return Indent(indentLevel + 1) + $"<label class=\"control-label\">{member.GetName()}</label>";
			yield return Indent(indentLevel + 1) + $"<input class=\"sr-only\" asp-for=\"{prefix}{member?.Name}.{nameof(IResourceReference.Type)}\"/>";
			yield return Indent(indentLevel + 1) + $"<a class=\"btn btn-primary\" role=\"button\" href=\"../../{urlStr}\">{nameStr}</a>";
			yield return Indent(indentLevel) + $"</div>";
		}

		private static IEnumerable<string> AdminDataView(string prefix, MemberInfo member, int indentLevel)
		{
			yield return Indent(indentLevel) + $"<div class=\"card\">";
			yield return Indent(indentLevel + 2) + $"<div class=\"card-body\">";
			yield return Indent(indentLevel + 1) + $"<h5 class=\"card-title\">{member.GetName()}</h5>";
			yield return Indent(indentLevel + 2) + $"@foreach(var admin in Model.{prefix}{member.Name}.Administrators) {{";
			// iterate admin array

			yield return Indent(indentLevel) + $"<div class=\"card\">";
			yield return Indent(indentLevel) + $"<div class=\"card\">";
			var name = $"@{prefix}admin.{nameof(AdministratorEntry.User)}.{nameof(IResourceReference.Name)}";
			var slug = $"@{prefix}admin.{nameof(AdministratorEntry.User)}.{nameof(IResourceReference.Slug)}";
			yield return Indent(indentLevel + 1) + $"<p>{name} [{slug}]</p>";
			yield return Indent(indentLevel) + $"</div>";
			yield return Indent(indentLevel) + $"</div>";

			yield return Indent(indentLevel + 2) + "}";
			yield return Indent(indentLevel + 2) + $"</div>";
			yield return Indent(indentLevel) + $"</div>";
		}

		private static IEnumerable<string> GetLocationEditor(string prefix, MemberInfo member, int indentLevel)
		{
			var name = member.Name;
			var template = File.ReadAllText("LocationPicker.template.cshtml");
			template = template.Replace("{LOCATION}", $"Model.{prefix}{name}");
			template = template.Replace("{ LOCATION }", $"Model.{prefix}{name}");
			yield return template;
		}

		protected static string Indent(int indent)
		{
			return new string('\t', indent);
		}

		protected static IEnumerable<string> BuildDataFields(Type targetType, int indentLevel, string prefix, bool forcereadonly = false)
		{
			foreach (var member in targetType.GetPropertiesAndFields(p => p.CanRead, f => true)
				.OrderBy(m => m.DeclaringType?.InheritanceHierarchy().Count()))
			{
				var viewAttr = member.GetCustomAttribute<ViewAttribute>();
				if (viewAttr == null)
				{
					continue;
				}
				var memberType = member.GetMemberType();
				if (memberType.IsInterface)
				{
					continue;
				}
				if (typeof(IDecryptable).IsAssignableFrom(memberType))
				{
					memberType = memberType.InheritanceHierarchy().First(t => t.IsGenericType).GetGenericArguments().First();
				}
				var typeName = memberType.GetRealTypeName(true);
				var memberName = member.Name;

				var callback = m_customTypeCallback.FirstOrDefault(kvp => kvp.Key.IsAssignableFrom(memberType)).Value;
				if (callback != null)
				{
					foreach (var line in callback(prefix, member, indentLevel))
					{
						yield return line;
					}
					continue;
				}

				bool isReadonly = forcereadonly || viewAttr.EditPermission > EPermissionLevel.ADMIN || (member is PropertyInfo p && !p.CanWrite);

				if (!memberType.IsPrimitive && !memberType.IsEnum && !memberType.Namespace.StartsWith(nameof(System)))
				{
					yield return Indent(indentLevel) + $"<div class=\"card\">";
					yield return Indent(indentLevel + 1) + $"<div class=\"card-body\">";
					yield return Indent(indentLevel + 2) + $"<h5 class=\"card-title\">{member.GetName()}</h5>";
					foreach (var line in BuildDataFields(memberType, indentLevel + 3, $"{prefix}{memberName}.", isReadonly))
					{
						yield return line;
					}
					yield return Indent(indentLevel + 2) + $"</div>";
					yield return Indent(indentLevel) + $"</div>";
				}
				else
				{
					string inputType = "input";
					if (member.GetCustomAttribute<DescriptionAttribute>() != null)
					{
						inputType = "textarea";
					}
					string inputExtras = "";
					if (member.GetCustomAttribute<PasswordAttribute>() != null)
					{
						inputExtras += "type=\"password\"";
					}
					if (isReadonly)
					{
						inputExtras += " readonly";
					}
					yield return Indent(indentLevel) + $"<div class=\"form-group\">";
					yield return Indent(indentLevel + 1) + $"<label asp-for=\"{prefix}{memberName}\" class=\"control-label\"></label>";
					yield return Indent(indentLevel + 1) + $"<{inputType} {inputExtras} asp-for=\"{prefix}{memberName}\" class=\"form-control\"></{inputType}>";
					yield return Indent(indentLevel + 1) + $"<span asp-validation-for=\"{prefix}{memberName}\" class=\"text-danger\"></span>";
					yield return Indent(indentLevel) + $"</div>";
				}
			}
		}
	}
}
