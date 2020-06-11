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
	public static class DeleteViewGenerator
	{
		public static string Generate(Type resourceType)
		{
			var template = File.ReadAllText("Delete.template");
			template = template.Replace("{TYPE}", resourceType.Name);
			template = template.Replace("{NAME}", resourceType.GetName());
			return template;
		}
	}
}
