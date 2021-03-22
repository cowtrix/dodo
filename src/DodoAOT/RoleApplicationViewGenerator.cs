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
			return template;
		}
	}
}
