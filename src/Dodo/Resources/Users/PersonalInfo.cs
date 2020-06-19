using Common.Extensions;
using Dodo.LocalGroups;
using Resources;
using System;

namespace Dodo.Users
{
	public class PersonalDataAttribute : Attribute { }

	public class PersonalInfo
	{
		[PersonalData]
		[Email]
		[View(EPermissionLevel.OWNER)]
		public string Email;
		[View(EPermissionLevel.OWNER)]
		public bool EmailConfirmed { get; set; }
		[View(EPermissionLevel.OWNER)]
		public ResourceReference<LocalGroup> LocalGroup { get; set; }
		[View(EPermissionLevel.OWNER)]
		public TimeZoneInfo Timezone { get; set; }
	}
}
