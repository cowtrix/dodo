using Common;
using Microsoft.AspNetCore.Mvc;
using Resources;
using System;

namespace Dodo
{
	[BindProperties]	
	public class AdministratorPermissionSet
	{
		public AdministratorPermissionSet() { }

		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Can Make Announcements")]
		public bool CanMakeAnnouncements { get; set; }

		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Can Make New Events")]
		public bool CanMakeNewEvents { get; set; }

		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Can Make New Sites")]
		public bool CanMakeNewSites { get; set; }

		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Can Edit Administrators")]
		public bool CanEditAdministrators { get; set; }

		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Can Delete")]
		public bool CanDelete { get; set; }

		public override bool Equals(object obj)
		{
			return obj is AdministratorPermissionSet set &&
				   CanEditAdministrators == set.CanEditAdministrators &&
				   CanDelete == set.CanDelete;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(CanEditAdministrators, CanDelete);
		}
	}
}
