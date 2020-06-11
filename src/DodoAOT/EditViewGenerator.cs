using Resources.Security;
using System;
using Common;
using System.Text;
using System.IO;

namespace DodoAOT
{

	public class EditViewGenerator : ViewGeneratorBase
	{
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
