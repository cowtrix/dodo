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
using System.Collections;
using System.Security.Cryptography.X509Certificates;

namespace DodoAOT
{
	public class ViewGeneratorBase
	{
		protected static string Template(string templateName)
		{
			var fullPath = Path.GetFullPath($"./Templates/{templateName}.template.cshtml");
			return File.ReadAllText(fullPath);
		}

		private delegate IEnumerable<string> CustomDrawerCallback(string prefix, MemberInfo member, int indentLevel);
		private static Dictionary<Type, CustomDrawerCallback> m_customTypeCallback = new Dictionary<Type, CustomDrawerCallback>()
		{
			{ typeof(AdministrationData), Null },
			{ typeof(GeoLocation), GetLocationEditor },
			{ typeof(IResourceReference), RefView },
			{ typeof(IList), ListView },
			{ typeof(Dodo.LocationResources.SiteFacilities), Facilities },
		};

		private static Dictionary<string, CustomDrawerCallback> m_customExcplicitCallback = new Dictionary<string, CustomDrawerCallback>()
		{
			{ "parentRef", RefView },
			{ "parentRefString", ParentRefFromStringDisplay },
			{ "markdown", MarkdownEditor },
			{ "null", Null },
			{ "published", Published },
		};

		private static IEnumerable<string> Published(string prefix, MemberInfo member, int indentLevel)
		{
			yield return Template("Published").Replace("{MEMBER}", $"{prefix}{member.Name}");
		}

		private static IEnumerable<string> ParentRefFromStringDisplay(string prefix, MemberInfo member, int indentLevel)
		{
			var template = Template("ParentRefStr");
			yield return template.Replace("{NAME}", member.GetName())
				.Replace("{MEMBER}", $"{prefix}{member.Name}");
		}

		private static IEnumerable<string> Facilities(string prefix, MemberInfo member, int indentLevel)
		{
			yield return Template("Facilities").Replace("{NAME}", member.GetName())
				.Replace("{MEMBER}", $"{prefix}{member.Name}");
		}

		private static IEnumerable<string> ListView(string prefix, MemberInfo member, int indentLevel)
		{
			var typeMember = member.GetMemberType().GetGenericArguments().First();
			var template = Template("List");
			var sb = new StringBuilder();
			foreach (var line in BuildDataField(typeMember, indentLevel + 3, $"{prefix}{member.Name}[i]."))
			{
				sb.AppendLine(line);
			}
			yield return template.Replace("{MEMBER}", $"{prefix}{member.Name}")
				.Replace("{TYPE}", typeMember.GetRealTypeName(true))
				.Replace("{REFTYPE}", typeMember.GetGenericArguments().First().Name.ToLowerInvariant())
				.Replace("{FIELD}", sb.ToString())
				.Replace("{NAME}", member.GetName());
		}

		private static IEnumerable<string> MarkdownEditor(string prefix, MemberInfo member, int indentLevel)
		{
			// Nah not good enough, plaintext for now...
			/*var txt = Template("MarkdownEditor");
			txt = txt.Replace("{NAME}", $"{prefix}{member.Name}");
			yield return txt;*/
			var view = member.GetCustomAttribute<ViewAttribute>();
			yield return Indent(indentLevel) + "<div class=\"form-field form-group\">";
			yield return Indent(indentLevel) + GetHelp(member.Name);
			yield return Indent(indentLevel) + $"<label class=\"control-label\">{member.GetName()}</label>";
			yield return Indent(indentLevel) + $"<textarea asp-for=\"{prefix}{member.Name}\" class=\"form-control description-entry\"></textarea>";
			if(!string.IsNullOrEmpty(view.InputHint))
			{
				yield return Indent(indentLevel + 1) + $"<small id=\"helpBlock\" class=\"form-text text-muted\">";
				yield return Indent(indentLevel + 2) + view.InputHint;
				yield return Indent(indentLevel + 1) + $"</small>";
			}
			yield return Indent(indentLevel) + "</div>";
		}

		private static IEnumerable<string> ParentRefDisplay(string prefix, MemberInfo member, int indentLevel)
		{
			var refType = member.GetMemberType().GetGenericArguments().First();
			var template = Template("ParentRef");
			yield return template.Replace("{NAME}", member.GetName())
				.Replace("{MEMBER}", $"{prefix}{member.Name}")
				.Replace("{TYPE}", $"{refType.Namespace}.{refType.Name}");
		}

		private static IEnumerable<string> Null(string prefix, MemberInfo member, int indentLevel)
		{
			yield return Indent(indentLevel + 1) + $"<input asp-for=\"{prefix}{member.Name}\" class=\"sr-only\"></input>"; ;
		}

		protected static string GetAdminEditor(Type resourceType)
		{
			if (!typeof(IAdministratedResource).IsAssignableFrom(resourceType))
			{
				return "";
			}
			var field = resourceType.GetProperty("AdministratorData");
			var template = Template("Admin");
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
			var template = Template("Notifications");
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
			var memberType = member.GetMemberType().GetGenericArguments().Single();
			if (memberType == typeof(User))
			{
				throw new Exception("Can't link to user profiles");
			}
			if (member is Type)
			{
				member = null;
			}
			var memberName = member != null ? $"{member.Name}." : "";
			var nameStr = $"@Model.{prefix}{memberName}{nameof(IResourceReference.Name)}";
			var urlStr = $"@Model.{prefix}{memberName}{nameof(IResourceReference.Type)}/@Model.{prefix}{memberName}{nameof(IResourceReference.Slug)}";
			var salt = Guid.NewGuid().ToString().Replace("-", "");
			if (member != null)
			{
				yield return Indent(indentLevel + 1) + $"<label class=\"control-label\">{member.GetName()}</label>";
				yield return Indent(indentLevel + 1) + GetHelp($"{member.Name}");
				salt = member?.GetHashCode().ToString();
			}
			yield return Indent(indentLevel + 1) + $"<input class=\"sr-only\" asp-for=\"{prefix}{memberName}{nameof(IResourceReference.Type)}\"/>";
			yield return Indent(indentLevel + 1) + $"@{{ var reftype{salt} = @Model.{prefix}{memberName}Type.ToLowerInvariant(); }}";
			yield return Indent(indentLevel + 1) + "<div class=\"row\">";
			yield return Indent(indentLevel + 1) + $"<div class=\"col\"><strong>{nameStr}</strong></div>";
			yield return Indent(indentLevel + 1) + $"<div class=\"col-auto\"><a class=\"btn btn-light @(reftype{salt})-reference\" role=\"button\" href=\"../../{urlStr}\"><i class=\"fa fa-eye\"></i>View</a></div>";
			yield return Indent(indentLevel + 1) + $"<div class=\"col-auto\"><a class=\"btn btn-light @(reftype{salt})-reference\" role=\"button\" href=\"../../edit/{urlStr}\"><i class=\"fa fa-edit\"></i>Edit</a></div>";
			yield return Indent(indentLevel + 1) + "</div>";
		}

		static string GetHelp(string key)
		{
			if(!HelpUtility.HasHelp(key))
			{
#if !DEBUG
				return "";
#endif
				return $"<button type =\"button\" class=\"help-button\" data-toggle=\"tooltip\" data-html=\"true\" title=\"No Help Specified\"><i class=\"fas fa-question-circle\"></i></button>";
			}
			return $"<button type =\"button\" class=\"help-button\" data-toggle=\"tooltip\" data-html=\"true\" title=\"@(Dodo.HelpUtility.GetHelpHTML($\"{key}\"))\"><i class=\"fas fa-question-circle\"></i></button>";
		}

		private static IEnumerable<string> GetLocationEditor(string prefix, MemberInfo member, int indentLevel)
		{
			var name = member.Name;
			var template = Template("LocationPicker");
			template = template.Replace("{LOCATION}", $"Model.{prefix}{name}");
			template = template.Replace("{ LOCATION }", $"Model.{prefix}{name}");
			yield return template;
		}

		protected static string Indent(int indent)
		{
			return new string('\t', indent);
		}

		public static IEnumerable<string> BuildDataField(MemberInfo member, int indentLevel, string prefix, bool forcereadonly = false)
		{
			var viewAttr = member.GetCustomAttribute<ViewAttribute>();
			var memberType = member.GetMemberType();
			if (memberType.IsInterface)
			{
				yield break;
			}
			if (typeof(IDecryptable).IsAssignableFrom(memberType))
			{
				memberType = memberType.InheritanceHierarchy().First(t => t.IsGenericType).GetGenericArguments().First();
			}
			var callback = m_customExcplicitCallback.FirstOrDefault(kvp => kvp.Key == viewAttr?.CustomDrawer).Value ??
				m_customTypeCallback.FirstOrDefault(kvp => kvp.Key.IsAssignableFrom(memberType)).Value;
			if (callback != null)
			{
				foreach (var line in callback(prefix, member, indentLevel))
				{
					yield return line;
				}
				yield break;
			}
			bool isReadonly = forcereadonly ||
				(viewAttr != null && viewAttr.EditPermission > EPermissionLevel.ADMIN) ||
				(member is PropertyInfo p && !p.CanWrite);
			var typeName = memberType.GetRealTypeName(true);
			var memberName = member.Name;
			if (!memberType.IsPrimitive && !memberType.IsEnum && !memberType.Namespace.StartsWith(nameof(System)))
			{
				yield return Indent(indentLevel) + $"<div class=\"form-field card\">";
				yield return Indent(indentLevel + 1) + $"<div class=\"card-body\">";
				yield return Indent(indentLevel + 1) + GetHelp(member.Name);
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
				var divId = memberName;
				bool swapOrder = false;

				if (member.GetMemberType().IsEnum)
				{
					inputType = $"select";
					inputExtras += $"asp-items=\"@Html.GetEnumSelectList<{member.GetMemberType().Namespace}.{member.GetMemberType().Name}>()\"";
				}
				else if (member.GetMemberType() == typeof(bool))
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
					//inputClass = "form-control-plaintext";
				}
				yield return Indent(indentLevel) + $"<div class=\"form-field {divClass}\">";
				yield return Indent(indentLevel + 1) + GetHelp(member.Name);
				var labelLine = Indent(indentLevel + 1) + $"<label class=\"{labelClass}\">{member.GetName()}</label>";
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
				if (!string.IsNullOrEmpty(viewAttr?.InputHint))
				{
					yield return Indent(indentLevel + 1) + $"<small id=\"helpBlock\" class=\"form-text text-muted\">";
					yield return Indent(indentLevel + 2) + viewAttr.InputHint;
					if (viewAttr.CustomDrawer == "slugPreview")	// TODO move to input hint
					{
						yield return Indent(indentLevel + 2) + "<p id=\"slugPreview\"></p>";
						yield return Indent(indentLevel + 2) + "<script>";
						yield return Indent(indentLevel + 3) + "$('form :input').change(function(){";
						yield return Indent(indentLevel + 4) + $"var inputVal = $('#{memberName}').val().toLowerCase();";
						yield return Indent(indentLevel + 4) + "inputVal = inputVal.split(new RegExp(\"[^a-z0-9_]\")).join('');";
						yield return Indent(indentLevel + 4) + "$('#slugPreview').text('URL will be /' + inputVal + '/')";
						yield return Indent(indentLevel + 3) + "});";
						yield return Indent(indentLevel + 2) + "</script>";
					}
					yield return Indent(indentLevel + 1) + $"</small>";
				}
				yield return Indent(indentLevel) + $"</div>";
			}
		}

		protected static IEnumerable<string> BuildDataFields(Type targetType, int indentLevel, string prefix, bool forcereadonly = false, bool ignoreView = false)
		{
			foreach (var member in targetType.GetPropertiesAndFields(p => p.CanRead, f => true)
				.Where(m => m.GetCustomAttribute<ViewAttribute>() != null)
				.OrderBy(m =>
				{
					var view = m.GetCustomAttribute<ViewAttribute>();
					return ignoreView || view != null ? view.Priority : 255;
				})
				.ThenBy(m => m.DeclaringType?.InheritanceHierarchy().Count()))
			{
				foreach (var l in BuildDataField(member, indentLevel, prefix, forcereadonly))
					yield return l;
			}
		}
	}
}
