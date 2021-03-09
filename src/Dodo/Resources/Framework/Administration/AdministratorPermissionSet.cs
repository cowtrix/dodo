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
		[Name("Edit Administrators")]
		public bool CanEditAdministrators { get; set; }

		public override bool Equals(object obj)
		{
			return obj is AdministratorPermissionSet set &&
				   CanEditAdministrators == set.CanEditAdministrators;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(CanEditAdministrators);
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
