using Resources.Security;
using System;
using Common;
using System.Text;
using System.IO;
using Resources;
using Dodo.Users.Tokens;
using System.Collections.Generic;
using Dodo.Roles;

namespace DodoAOT
{
	public class EditViewGenerator : ViewGeneratorBase
	{
		public static string Generate(Type resourceType)
		{
			var template = Template("Edit");
			var sb = new StringBuilder();
			foreach(var s in BuildScripts())
			{
				sb.AppendLine("<script>");
				sb.AppendLine(s);
				sb.AppendLine("</script>");
			}
			template = template.Replace("{SCRIPTS}", sb.ToString());
			sb.Clear();
			foreach (var line in BuildDataFields(resourceType, 4, ""))
			{
				sb.AppendLine(line);
			}
			var viewBody = sb.ToString();
			template = template.Replace("{DETAILS}", viewBody);
			template = template.Replace("{NOTIFICATIONS}", GetNotificationEditor(resourceType));
			template = template.Replace("{ADMIN}", string.Join('\n', GetAdminEditor(resourceType)));
			if (resourceType == typeof(Dodo.Roles.Role))
			{
				template = template.Replace("{ROLES}", GetApplicationsEditor());
			}

			var schemaType = ResourceUtility.GetFactory(resourceType).SchemaType;
			template = template.Replace("{SCHEMA_TYPE}", schemaType.FullName);
			template = template.Replace("{TYPE}", resourceType.Name);
			template = template.Replace("{FULL_TYPE}", $"{resourceType.Namespace}.{resourceType.Name}");
			template = template.Replace("{NAME}", resourceType.GetName());
					
			return template;
		}

		private static string GetApplicationsEditor()
		{
			var field = typeof(Role).GetProperty(nameof(Role.Applications));
			var template = Template("Applications");
			template = template.Replace("{NAME}", field.GetName());
			template = template.Replace("{MEMBER}", field.GetName());
			return template;
		}
	}
}
