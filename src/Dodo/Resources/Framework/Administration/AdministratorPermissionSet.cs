using Common;
using Microsoft.AspNetCore.Mvc;
using Resources;

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
		[Name("Create Announcements")]
		public bool CanCreateAnnouncements { get; set; }
	}
}
