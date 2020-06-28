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
		private delegate IEnumerable<string> CustomDrawerCallback(string prefix, MemberInfo member, int indentLevel);
		private static Dictionary<Type, CustomDrawerCallback> m_customTypeCallback = new Dictionary<Type, CustomDrawerCallback>()
		{
			{ typeof(AdministrationData), Null },
			{ typeof(GeoLocation), GetLocationEditor },
			{ typeof(IResourceReference), RefView },
		};
		private static Dictionary<string, CustomDrawerCallback> m_customExcplicitCallback = new Dictionary<string, CustomDrawerCallback>()
		{
			{ "parentRef", ParentRefDisplay }
		};

		private static IEnumerable<string> ParentRefDisplay(string prefix, MemberInfo member, int indentLevel)
		{
			var memberType = member.GetMemberType();
			var memberName = member.Name;
			var refType = memberType.GetGenericArguments().First();
			yield return Indent(indentLevel) + $"@{{ var rscColor = @Dodo.APIController.TypeDisplayColors[Model.{prefix}{memberName}.GetRefType()]; }}";
			yield return Indent(indentLevel) + $"<div class=\"navbar navbar-expand-lg navbar-dark\" style=\"width=100%; background-color:#@rscColor; margin:-20px; margin-bottom:20px;\">";
			yield return Indent(indentLevel) + $"<a class=\"navbar-brand\" href=\"{Dodo.DodoApp.NetConfig.FullURI}/@Model.{prefix}{memberName}.GetRefType().Name/@Model.{prefix}{memberName}.Slug\">Part of the @Model.{prefix}{memberName}.Name</a>";
			yield return Indent(indentLevel) + $"</div>";
		}

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
				.OrderBy(m =>
				{
					var view = m.GetCustomAttribute<ViewAttribute>();
					return view != null ? view.Priority : 255;
				})
				.ThenBy(m => m.DeclaringType?.InheritanceHierarchy().Count()))
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

				var callback = m_customExcplicitCallback.FirstOrDefault(kvp => kvp.Key == viewAttr.CustomDrawer).Value ??
					m_customTypeCallback.FirstOrDefault(kvp => kvp.Key.IsAssignableFrom(memberType)).Value;
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
					string inputExtras = "";
					string inputClass = "form-control";
					var labelClass = "control-label";
					var divClass = "form-group";
					bool swapOrder = false;
					
					if (member.GetCustomAttribute<DescriptionAttribute>() != null)
					{
						inputType = "textarea";
						inputExtras += " data-provide=\"markdown\" rows=\"10\" data-resize=\"vertical\" data-iconlibrary=\"fa\"";
					}
					else if (member.GetMemberType().IsEnum)
					{
						inputType = $"select";
						inputExtras += $"asp-items=\"@Html.GetEnumSelectList<{member.GetMemberType().Namespace}.{member.GetMemberType().Name}>()\"";
					}
					else if(member.GetMemberType() == typeof(bool))
					{
						labelClass = "form-check-label";
						inputExtras += "type=\"checkbox\"";
						inputClass = "form-check-input";
						divClass = "form-check";
						swapOrder = true;
					}
					else if (member.GetCustomAttribute<PasswordAttribute>() != null)
					{
						inputExtras += "type=\"password\"";
					}
					if (isReadonly)
					{
						inputExtras += " readonly";
						inputClass += " .form-control-plaintext";
					}
					yield return Indent(indentLevel) + $"<div class=\"{divClass}\">";
					var labelLine = Indent(indentLevel + 1) + $"<label asp-for=\"{prefix}{memberName}\" class=\"{labelClass}\"></label>";
					var inputLine = Indent(indentLevel + 1) + $"<{inputType} {inputExtras} asp-for=\"{prefix}{memberName}\" class=\"{inputClass}\"></{inputType}>";
					if (swapOrder)
					{
						yield return inputLine;
						yield return labelLine;
					}
					else
					{
						yield return labelLine;
						yield return inputLine;
					}					
					yield return Indent(indentLevel + 1) + $"<span asp-validation-for=\"{prefix}{memberName}\" class=\"text-danger\"></span>";
					if(!string.IsNullOrEmpty(viewAttr.InputHint))
					{
						yield return Indent(indentLevel + 1) + $"<small id=\"helpBlock\" class=\"form-text text-muted\">";
						yield return Indent(indentLevel + 2) + viewAttr.InputHint;
						yield return Indent(indentLevel + 1) + $"</small>";
					}
					yield return Indent(indentLevel) + $"</div>";
				}
			}
		}
	}
}
