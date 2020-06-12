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

namespace DodoAOT
{
	public class ViewGeneratorBase
	{
		private delegate IEnumerable<string> CustomTypeCallback(string prefix, MemberInfo member, int indentLevel);
		private static Dictionary<Type, CustomTypeCallback> m_customTypeCallback = new Dictionary<Type, CustomTypeCallback>()
		{
			//{ typeof(GeoLocation), GetLocationEditor },
			{ typeof(LocationData), LocationDataView },
			{ typeof(Dodo.GroupResource.AdminData), AdminDataView },
			{ typeof(IResourceReference), RefView },
		};

		private static IEnumerable<string> RefView(string prefix, MemberInfo member, int indentLevel)
		{
			yield return Indent(indentLevel) + $"<div class=\"card\">";
			yield return Indent(indentLevel) + $"<label>@{prefix}{nameof(IResourceReference.Name)}</label>";
			yield return Indent(indentLevel) + $"<label>@{prefix}{nameof(IResourceReference.Slug)}</label>";
			yield return Indent(indentLevel) + $"<label>@{prefix}{nameof(IResourceReference.Guid)}</label>";
			yield return Indent(indentLevel) + $"<label>@{prefix}{nameof(IResourceReference.Type)}</label>";
			yield return Indent(indentLevel + 2) + $"</div>";
		}

		private static IEnumerable<string> AdminDataView(string prefix, MemberInfo member, int indentLevel)
		{
			yield return Indent(indentLevel) + $"<div class=\"card\">";
			yield return Indent(indentLevel + 1) + $"<h5 class=\"card-title\">{member.GetName()}</h5>";
			yield return Indent(indentLevel + 2) + $"<div class=\"card-body\">";
			yield return Indent(indentLevel + 2) + $"@foreach(var admin in Model.{prefix}{member.Name}.Administrators) {{";
			// iterate admin array
			foreach (var l in RefView("admin.", member, indentLevel + 3)) yield return l;
			yield return Indent(indentLevel + 2) + "}";
			yield return Indent(indentLevel + 2) + $"</div>";
			yield return Indent(indentLevel) + $"</div>";
		}

		private static IEnumerable<string> LocationDataView(string prefix, MemberInfo member, int indentLevel)
		{
			var name = member.Name;
			IEnumerable<string> labelIfNotNull(string fieldName)
			{
				yield return Indent(indentLevel + 1) + $"@if (!string.IsNullOrEmpty(Model.{prefix}{name}.{fieldName})) {{";
				yield return Indent(indentLevel + 2) + $"<br>@Model.{prefix}{name}.{fieldName}";
				yield return Indent(indentLevel + 1) + "}";
			}
			yield return Indent(indentLevel) + $"<address>";
			yield return Indent(indentLevel + 1) + $"<strong>Address</strong>";
			foreach (var l in labelIfNotNull(nameof(LocationData.Address))) yield return l;
			foreach (var l in labelIfNotNull(nameof(LocationData.Neighborhood))) yield return l;
			foreach (var l in labelIfNotNull(nameof(LocationData.Locality))) yield return l;
			foreach (var l in labelIfNotNull(nameof(LocationData.Place))) yield return l;
			foreach (var l in labelIfNotNull(nameof(LocationData.District))) yield return l;
			foreach (var l in labelIfNotNull(nameof(LocationData.Postcode))) yield return l;
			foreach (var l in labelIfNotNull(nameof(LocationData.Region))) yield return l;
			foreach (var l in labelIfNotNull(nameof(LocationData.Country))) yield return l;
			yield return Indent(indentLevel) + $"</address>";
		}

		protected static string Indent(int indent)
		{
			return new string('\t', indent);
		}

		protected static IEnumerable<string> BuildClass(Type targetType, int indentLevel, string prefix, bool forcereadonly = false)
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

				if (m_customTypeCallback.TryGetValue(memberType, out var callback))
				{
					foreach (var line in callback(prefix, member, indentLevel))
					{
						yield return line;
					}
					continue;
				}

				bool isReadonly = forcereadonly || viewAttr.EditPermission > EPermissionLevel.ADMIN || (member is PropertyInfo p && !p.CanWrite);

				if (!memberType.IsPrimitive && !memberType.Namespace.StartsWith(nameof(System)))
				{
					yield return Indent(indentLevel) + $"<div class=\"card\">";
					yield return Indent(indentLevel + 1) + $"<div class=\"card-body\">";
					yield return Indent(indentLevel + 2) + $"<h5 class=\"card-title\">{member.GetName()}</h5>";
					foreach (var line in BuildClass(memberType, indentLevel + 3, $"{prefix}{memberName}.", isReadonly))
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
					if (isReadonly)
					{
						inputExtras += " readonly";
					}
					yield return Indent(indentLevel) + $"<div class=\"form-group\">";
					yield return Indent(indentLevel + 1) + $"<label asp-for=\"{prefix}{memberName}\" class=\"control-label\"></label>";
					yield return Indent(indentLevel + 1) + $"<input {inputExtras} asp-for=\"{prefix}{memberName}\" class=\"form-control\"/>";
					yield return Indent(indentLevel + 1) + $"<span asp-validation-for=\"{prefix}{memberName}\" class=\"text-danger\"></span>";
					yield return Indent(indentLevel) + $"</div>";
				}
			}
		}
	}
}
