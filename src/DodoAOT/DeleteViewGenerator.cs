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
	public class DeleteViewGenerator : ViewGeneratorBase
	{
		public static string Generate(Type resourceType)
		{
			var template = Template("Delete");
			template = template.Replace("{TYPE}", resourceType.Name);
			template = template.Replace("{NAME}", resourceType.GetName());
			return template;
		}
	}
}
