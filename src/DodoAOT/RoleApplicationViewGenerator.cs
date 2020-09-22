using Dodo.RoleApplications;
using Common;
using System;
using System.Text;

namespace DodoAOT
{
	public class RoleApplicationViewGenerator : ViewGeneratorBase
	{
		public static string Generate()
		{
			var template = Template("Application");
			var sb = new StringBuilder();
			foreach (var s in BuildScripts())
			{
				sb.AppendLine("<script>");
				sb.AppendLine(s);
				sb.AppendLine("</script>");
			}
			template = template.Replace("{SCRIPTS}", sb.ToString());
			return template;
		}
	}
}
