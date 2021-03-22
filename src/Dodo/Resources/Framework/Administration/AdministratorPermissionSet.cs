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
