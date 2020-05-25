using System.Collections.Generic;

namespace DodoServer.ViewModels
{
	/// <summary>
	/// View Model for working group containing both read-only and writable properties
	/// </summary>
	public class WorkingGroup : Group
	{
		public List<string> Roles { get; set; }
		public List<string> WorkingGroups { get; set; }
	}
}
