using Common;
using Microsoft.AspNetCore.Mvc;
using Resources;
using System;
using System.Collections.Generic;

namespace Dodo
{
	[BindProperties]	
	public class AdministratorPermissionSet
	{
		public AdministratorPermissionSet() { }

		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Change Administrator Permissions")]
		public bool CanChangePermissions { get; set; }
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Add New Administrators")]
		public bool CanAddAdmin { get; set; }
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Remove Existing Administrators")]
		public bool CanRemoveAdmin { get; set; }
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Edit Information")]
		public bool CanEditInfo { get; set; }
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Create New Child Objects")]
		public bool CanCreateChildObjects { get; set; }
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Delete Existing Child Objects")]
		public bool CanDeleteChildObjects { get; set; }
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[Name("Manage Announcements")]
		public bool CanManageAnnouncements { get; set; }

		public override bool Equals(object obj)
		{
			return obj is AdministratorPermissionSet set &&
				   CanChangePermissions == set.CanChangePermissions &&
				   CanAddAdmin == set.CanAddAdmin &&
				   CanRemoveAdmin == set.CanRemoveAdmin &&
				   CanEditInfo == set.CanEditInfo &&
				   CanCreateChildObjects == set.CanCreateChildObjects &&
				   CanDeleteChildObjects == set.CanDeleteChildObjects &&
				   CanManageAnnouncements == set.CanManageAnnouncements;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(CanChangePermissions, CanAddAdmin, CanRemoveAdmin, CanEditInfo, CanCreateChildObjects, CanDeleteChildObjects, CanManageAnnouncements);
		}

		public static bool operator ==(AdministratorPermissionSet left, AdministratorPermissionSet right)
		{
			return EqualityComparer<AdministratorPermissionSet>.Default.Equals(left, right);
		}

		public static bool operator !=(AdministratorPermissionSet left, AdministratorPermissionSet right)
		{
			return !(left == right);
		}
	}
}
