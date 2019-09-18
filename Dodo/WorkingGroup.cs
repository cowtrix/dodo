using Newtonsoft.Json;
using System.Collections.Generic;

namespace XR.Dodo
{
	public enum EParentGroup
	{
		ActionSupport,
		ArresteeSupport,
		WorldBuildingProd,
		MediaAndMessaging,
		MovementSupport,
		RSO,
	}

	public struct WorkingGroup
	{
		public string Name;
		public string Mandate;
		public EParentGroup ParentGroup;
		public string ShortCode;

		public WorkingGroup(string workingGroup, EParentGroup parentGroup, string mandate, string shortcode)
		{
			Name = workingGroup.Trim();
			ParentGroup = parentGroup;
			Mandate = mandate;
			ShortCode = shortcode;
		}
	}

	public class Role
	{
		public int SiteCode;
		public string Name;
		public WorkingGroup WorkingGroup;

		[JsonIgnore]
		public SiteSpreadsheet Site { get { return DodoServer.SiteManager.GetSite(SiteCode); } }

		public Role(WorkingGroup workingGroup, string name, int siteCode)
		{
			WorkingGroup = workingGroup;
			Name = name;
			SiteCode = siteCode;
		}
	}
}
