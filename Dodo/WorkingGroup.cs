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

		public override bool Equals(object obj)
		{
			var role = obj as Role;
			return role != null &&
				   SiteCode == role.SiteCode &&
				   Name == role.Name &&
				   WorkingGroup.ShortCode ==  role.WorkingGroup.ShortCode;
		}

		public override int GetHashCode()
		{
			var hashCode = -489653820;
			hashCode = hashCode * -1521134295 + SiteCode.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WorkingGroup.ShortCode);
			return hashCode;
		}
	}
}
