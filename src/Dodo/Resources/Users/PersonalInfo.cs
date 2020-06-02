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
	}
}
