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

		public bool EmailConfirmed { get; set; }

		[PhoneNumber]
		[PersonalData]
		[View(EPermissionLevel.OWNER)]
		public string PhoneNumber;

		public bool PhoneNumberConfirmed { get; set; }

		[View(EPermissionLevel.OWNER)]
		[PersonalData]
		public ResourceReference<LocalGroup> LocalGroup;
	}
}
