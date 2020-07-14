using Common.Extensions;
using Dodo.LocalGroups;
using Resources;
using System;

namespace Dodo.Users
{
	public class EmailPreferences
	{
		[View(EPermissionLevel.OWNER)]
		public bool WeeklyUpdate { get; set; }
		[View(EPermissionLevel.OWNER)]
		public bool DailyUpdate { get; set; }
		[View(EPermissionLevel.OWNER)]
		public bool NewNotifications { get; set; }
	}

	public class PersonalInfo
	{
		[Email]
		[View(EPermissionLevel.OWNER)]
		public string Email;
		[View(EPermissionLevel.OWNER)]
		public bool EmailConfirmed { get; set; }
		[View(EPermissionLevel.OWNER)]
		public ResourceReference<LocalGroup> LocalGroup { get; set; }
		[View(EPermissionLevel.OWNER)]
		public TimeZoneInfo Timezone { get; set; }
		[View(EPermissionLevel.USER)]
		[VerifyObject]
		public EmailPreferences EmailPreferences { get; set; } = new EmailPreferences();
	}
}
