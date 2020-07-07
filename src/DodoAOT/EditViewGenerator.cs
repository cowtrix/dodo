using Resources.Security;
using System;
using Common;
using System.Text;
using System.IO;
using Resources;
using Dodo.Users.Tokens;
using System.Collections.Generic;

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
			template = template.Replace("{TYPE}", resourceType.Name);
			template = template.Replace("{NAME}", resourceType.GetName());
			
			foreach (var line in BuildDataFields(resourceType, 4, ""))
			{
				sb.AppendLine(line);
			}
			var viewBody = sb.ToString();
			template = template.Replace("{DETAILS}", viewBody);
			template = template.Replace("{NOTIFICATIONS}", GetNotificationEditor(resourceType));
			template = template.Replace("{ADMIN}", string.Join('\n', GetAdminEditor(resourceType)));
			return template;
		}

		
	}
}
