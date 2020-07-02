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
	public class CreateViewGenerator : ViewGeneratorBase
	{
		public static string Generate(Type resourceType)
		{
			var schemaType = ResourceUtility.GetFactory(resourceType).SchemaType;
			var template = Template("Create");
			template = template.Replace("{SCHEMA_TYPE}", schemaType.FullName);
			template = template.Replace("{NAME}", resourceType.GetName());
			var view = new StringBuilder();
			foreach (var line in BuildDataFields(schemaType, 4, ""))
			{
				view.AppendLine(line);
			}
			var viewBody = view.ToString();

			template = template.Replace("{BODY}", viewBody);
			return template;
		}
	}
}
