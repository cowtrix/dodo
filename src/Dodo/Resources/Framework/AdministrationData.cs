using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Common;
using Dodo.Users;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo
{
	public class AdministratorEntry
	{
		public AdministratorEntry() { }
		public AdministratorEntry(User user)
		{
			User = user.CreateRef();
		}
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public ResourceReference<User> User { get; set; }
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public AdministratorPermissionSet Permissions { get; set; } = new AdministratorPermissionSet();
	}

	public class AdministratorPermissionSet
	{
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
	}

	public class AdministrationData
	{
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		[JsonProperty]
		public List<AdministratorEntry> Administrators = new List<AdministratorEntry>();
		[JsonProperty]
		public string GroupPrivateKey { get; private set; }
		[JsonProperty]
		ResourceReference<IRESTResource> Resource { get; set; }

		public AdministrationData() { }

		public AdministrationData(IRESTResource resource, User firstAdmin, string privateKey)
		{
			if (resource == null)
			{
				throw new ArgumentNullException(nameof(resource));
			}
			else if (firstAdmin == null)
			{
				throw new ArgumentNullException(nameof(firstAdmin));
			}
			else if (string.IsNullOrEmpty(privateKey))
			{
				throw new ArgumentNullException(nameof(privateKey));
			}
			Resource = resource.CreateRef();
			Administrators.Add(new AdministratorEntry(firstAdmin)
			{
				Permissions = new AdministratorPermissionSet
				{
					// The first admin gets it all
					CanAddAdmin = true,
					CanRemoveAdmin = true,
					CanEditInfo = true,
					CanCreateChildObjects = true,
					CanDeleteChildObjects = true,
				}
			});
			GroupPrivateKey = privateKey;
		}

		internal bool AddOrUpdateAdministrator(AccessContext context, User newAdmin, AdministratorPermissionSet permissions = null)
		{
			if (context.User == newAdmin)
			{
				SecurityWatcher.RegisterEvent($"{Resource}: Admin {context.User}, tried to change their own permissions, which is never allowed.");
				return false;
			}
			var changingAdminEntry = Administrators.SingleOrDefault(ad => ad.User.Guid == context.User.Guid);
			if (changingAdminEntry == null)
			{
				SecurityWatcher.RegisterEvent($"{Resource}: Admin {context.User}, who isn't on the administrator list managed to decrypt it and tried to add {newAdmin} as a Administrator");
				return false;
			}
			if (!changingAdminEntry.Permissions.CanAddAdmin)
			{
				SecurityWatcher.RegisterEvent($"{Resource} : Admin {context.User} tried to add {newAdmin} as an admin, and they don't have permission to do so.");
				return false;
			}
			var alteredAdminEntry = Administrators.SingleOrDefault(ad => ad.User.Guid == newAdmin.Guid);
			if (alteredAdminEntry == null)
			{
				alteredAdminEntry = new AdministratorEntry(newAdmin);
				Administrators.Add(alteredAdminEntry);
			}
			if (permissions != null)
			{
				alteredAdminEntry.Permissions = permissions;
			}
			return true;
		}
	}
}
