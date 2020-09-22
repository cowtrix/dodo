using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Dodo.Users;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo
{
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

		public AdministrationData(IRESTResource resource, User firstAdmin, Passphrase privateKey)
		{
			if (resource == null)
			{
				throw new ArgumentNullException(nameof(resource));
			}
			else if (firstAdmin == null)
			{
				throw new ArgumentNullException(nameof(firstAdmin));
			}
			else if (string.IsNullOrEmpty(privateKey.Value))
			{
				throw new ArgumentNullException(nameof(privateKey));
			}
			Administrators.Add(new AdministratorEntry(firstAdmin)
			{
				Permissions = new AdministratorPermissionSet
				{
					// The first admin gets it all
					CanChangePermissions = true,
					CanAddAdmin = true,
					CanRemoveAdmin = true,
					CanEditInfo = true,
					CanCreateChildObjects = true,
					CanDeleteChildObjects = true,
					CanManageAnnouncements = true,
					CanManageRoles = true,
				}
			});
			GroupPrivateKey = privateKey.Value;
			Resource = resource.CreateRef();
		}

		internal bool AddOrUpdateAdministrator(AccessContext context, User newAdmin, AdministratorPermissionSet permissions = null)
		{
			if (context.User == newAdmin)
			{
				SecurityWatcher.RegisterEvent(context.User, $"{Resource}: Admin {context.User}, tried to change their own permissions, which is never allowed.");
				return false;
			}
			var adminMakingChange = Administrators.SingleOrDefault(ad => ad.User.Guid == context.User.Guid);
			if (adminMakingChange == null)
			{
				SecurityWatcher.RegisterEvent(context.User, $"{Resource}: Admin {context.User}, who isn't on the administrator list managed to decrypt it and tried to add {newAdmin} as a Administrator");
				return false;
			}
			var alteredAdminEntry = Administrators.SingleOrDefault(ad => ad.User.Guid == newAdmin.Guid);
			if (alteredAdminEntry == null)
			{
				if (!adminMakingChange.Permissions.CanAddAdmin)
				{
					SecurityWatcher.RegisterEvent(context.User, $"{Resource} : Admin {context.User} tried to add {newAdmin} as an admin, and they don't have permission to do so.");
					return false;
				}
				alteredAdminEntry = new AdministratorEntry(newAdmin);
				Administrators.Add(alteredAdminEntry);
			}
			if (permissions != null)
			{
				if (!adminMakingChange.Permissions.CanChangePermissions)
				{
					SecurityWatcher.RegisterEvent(context.User, $"{Resource} : Admin {context.User} tried to change permissions for {newAdmin}, and they don't have permission to do so.");
					return false;
				}
				alteredAdminEntry.Permissions = permissions;
			}
			return true;
		}
	}
}
