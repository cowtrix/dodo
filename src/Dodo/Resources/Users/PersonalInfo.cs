using Common.Extensions;
using Dodo.LocalGroups;
using REST;
using Microsoft.AspNetCore.Identity;

namespace Dodo.Users
{
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
