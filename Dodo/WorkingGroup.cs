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

		public override bool Equals(object obj)
		{
			return obj is WorkingGroup group &&
				   ShortCode == group.ShortCode;
		}

		public override int GetHashCode()
		{
			return -898233052 + EqualityComparer<string>.Default.GetHashCode(ShortCode);
		}
	}

	public class Role
	{
		public int SiteCode;
		public string Name;
		public string WorkingGroupCode;

		[JsonIgnore]
		public WorkingGroup WorkingGroup { get { return DodoServer.SiteManager.Data.WorkingGroups[WorkingGroupCode]; } }

		[JsonIgnore]
		public SiteSpreadsheet Site { get { return DodoServer.SiteManager.GetSite(SiteCode); } }

		public Role(WorkingGroup workingGroup, string name, int siteCode)
		{
			WorkingGroupCode = workingGroup.ShortCode;
			Name = name;
			SiteCode = siteCode;
		}

		public override bool Equals(object obj)
		{
			var role = obj as Role;
			return role != null &&
				   SiteCode == role.SiteCode &&
				   Name == role.Name &&
				   WorkingGroupCode ==  role.WorkingGroupCode;
		}

		public override int GetHashCode()
		{
			var hashCode = -489653820;
			hashCode = hashCode * -1521134295 + SiteCode.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WorkingGroupCode);
			return hashCode;
		}
	}
}
