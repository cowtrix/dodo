using System;
using Resources.Security;
using Dodo.Users;
using Resources;
using Common.Security;
using Dodo.Users.Tokens;
using Common;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Dodo
{
	public abstract class AdministratedGroupResource : GroupResource, IAdministratedResource, IParentResource
	{
		public AdministratedGroupResource() : base() { }
		public AdministratedGroupResource(AccessContext context, DescribedResourceSchemaBase schema) : base(context, schema)
		{
			// Creator gets super admin
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			m_publicKey = new Passphrase(pk);
			AdministratorData = new UserMultiSigStore<AdministrationData>(
				new AdministrationData(this, context.User, new Passphrase(pv)), context);
			using (var userLock = new ResourceLock(context.User))
			{
				var newAdmin = userLock.Value as User;
				newAdmin.TokenCollection.AddOrUpdate(newAdmin, new UserAddedAsAdminToken(this, newAdmin));
				ResourceUtility.GetManager<User>().Update(newAdmin, userLock);
			}
		}

		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public UserMultiSigStore<AdministrationData> AdministratorData { get; set; }

		#region Child Objects
		public abstract bool CanContain(Type type);

		public virtual void AddChild<T>(AccessContext context, T rsc) where T : class, IOwnedResource
		{
			/*TokenCollection.AddOrUpdate(this, new SimpleNotificationToken(null, null, $"A new {rsc.GetType().GetName()} was created: \"{rsc.Name}\"",
				$"{Dodo.DodoApp.NetConfig.FullURI}/{rsc.GetType().Name.ToLowerInvariant()}/{rsc.Slug}", ENotificationType.Alert, EPermissionLevel.ADMIN, false));*/
		}

		public virtual bool RemoveChild<T>(AccessContext context, T rsc) where T : class, IOwnedResource
		{
			TokenCollection.AddOrUpdate(this, new SimpleNotificationToken(null, null, $"The {rsc.GetType().GetName()} \"{rsc.Name}\" was deleted.",
				null, ENotificationType.Alert, EPermissionLevel.ADMIN, false));
			return true;
		}
		#endregion

		#region Administration
		/// <summary>
		/// Is the target user an administrator? Only another administrator can find out.
		/// </summary>
		/// <param name="target">The user to test</param>
		/// <param name="requesterContext">An existing administrator's context</param>
		/// <param name="permissions">If the target is an admin, their permissions</param>
		/// <returns>True on success, false on failure</returns>
		public bool IsAdmin(User target, AccessContext requesterContext, out AdministratorPermissionSet permissions)
		{
			permissions = null;
			var userRef = requesterContext.User.CreateRef();
			if (!AdministratorData.IsAuthorised(userRef, requesterContext.Passphrase))
			{
				return false;
			}
			// GetValue should never fail here
			var data = AdministratorData.GetValue(userRef, requesterContext.Passphrase);
			var entry = data.Administrators.SingleOrDefault(ad => ad.User.Guid == target.Guid);
			if (entry == null)
			{
				return false;
			}
			permissions = entry.Permissions;
			return true;
		}

		public bool RemoveAdmin(AccessContext context, User targetUser)
		{
			if (!IsAdmin(context.User, context, out var administratorPermission) || !administratorPermission.CanEditAdministrators)
			{
				// Context isn't admin, or doesn't have correct permissions
				SecurityWatcher.RegisterEvent(context.User, $"User {context.User} tried to remove {targetUser} as a new administrator for {this}, but they weren't an administrator.");
				return false;
			}
			var adminData = AdministratorData.GetValue(context.User.CreateRef(), context.Passphrase);
			adminData.Administrators = adminData.Administrators.Where(ad => ad.User.Guid != targetUser.Guid).ToList();
			AdministratorData.SetValue(adminData, context.User.CreateRef(), context.Passphrase);
			/*TokenCollection.AddOrUpdate(this, new EncryptedNotificationToken(context.User, null, $"Administrator @{context.User.Slug} removed @{targetUser.Slug} as an administrator",
				null, ENotificationType.Alert, EPermissionLevel.ADMIN, false));*/
			return true;
		}

		/// <summary>
		/// Add a new administrator to the resource.
		/// </summary>
		/// <param name="context">The existing administrator context who is adding the new administrator</param>
		/// <param name="newAdmin">The new administrator</param>
		/// <returns>True on success, false on failure</returns>
		public bool AddNewAdmin(AccessContext context, User newAdmin)
		{
			var newPass = new Passphrase(KeyGenerator.GetUniqueKey(32));
			if (newAdmin == null)
			{
				throw new ArgumentNullException(nameof(newAdmin));
			}
			if (!IsAdmin(context.User, context, out var administratorPermission) || !administratorPermission.CanEditAdministrators)
			{
				// Context isn't admin, or doesn't have correct permissions
				SecurityWatcher.RegisterEvent(context.User, $"User {context.User} tried to add {newAdmin} as a new administrator for {this}, but they weren't an administrator.");
				return false;
			}
			if (newAdmin.Guid == context.User.Guid || IsAdmin(newAdmin, context, out _))
			{
				// User is already admin
				return true;
			}
			var newAdminRef = newAdmin.CreateRef();
			AdministratorData.AddPermission(context.User.CreateRef(), context.Passphrase, newAdminRef, newPass);
			var adminData = AdministratorData.GetValue(newAdminRef, newPass);
			if (!adminData.AddOrUpdateAdministrator(context, newAdmin))
			{
				return false;
			}
			AdministratorData.SetValue(adminData, newAdminRef, newPass);
			/*TokenCollection.AddOrUpdate(this, new EncryptedNotificationToken(context.User, null,
				$"Administrator @{context.User.Slug} added new Administrator @{newAdmin.Slug}",
				null, ENotificationType.Alert, EPermissionLevel.ADMIN, false));*/
			Email.EmailUtility.SendEmail(new Email.EmailAddress { Name = newAdmin.Name, Email = newAdmin.PersonalData.Email },
				$"You have been added as an administrator on {Dodo.DodoApp.PRODUCT_NAME}",
				"Callback",
				new Dictionary<string, string>
				{
					{ "MESSAGE", $"You just been added as an administrator of the {GetType().GetName()} \"{Name}\" on {Dodo.DodoApp.PRODUCT_NAME}." },
					{ "CALLBACK_MESSAGE", "Visit Administration Panel" },
					{ "CALLBACK_URL", $"{Dodo.DodoApp.NetConfig.FullURI}/edit/{GetType().Name.ToLowerInvariant()}/{Slug}" }
				});
			using (var userLock = new ResourceLock(newAdmin))
			{
				newAdmin = userLock.Value as User;
				newAdmin.TokenCollection.AddOrUpdate(newAdmin, new UserAddedAsAdminToken(this.CreateRef<IAdministratedResource>(), newPass, newAdmin.AuthData.PublicKey, newAdmin));
				ResourceUtility.GetManager<User>().Update(newAdmin, userLock);
			}
			return true;
		}

		public bool CompleteAdminInvite(AccessContext context, Passphrase tempPass)
		{
			if (context.User == null)
			{
				throw new ArgumentNullException(nameof(context.User));
			}
			if (IsAdmin(context.User, context, out _))
			{
				// User has no pending invite
				return false;
			}
			var adminRef = context.User.CreateRef();
			AdministratorData.AddPermission(adminRef, tempPass, adminRef, context.Passphrase);
			var adminData = AdministratorData.GetValue(adminRef, context.Passphrase);
			if (!adminData.AddOrUpdateAdministrator(context, context.User))
			{
				return false;
			}
			AdministratorData.SetValue(adminData, adminRef, context.Passphrase);
#if DEBUG
			// Do a bit of extra testing just to make sure
			if (!IsAdmin(context.User, context, out _))
			{
				throw new Exception($"Failed to complete {context.User} Administrator invite");
			}
#endif
			return true;
		}

		public bool UpdateAdmin(AccessContext context, User target, AdministratorPermissionSet newPermissions)
		{
			string GetPermissionDiff(AdministratorPermissionSet old, AdministratorPermissionSet newp)
			{
				// This just generates a nice message of what permissions have changed
				var sb = new StringBuilder();
				foreach (var prop in typeof(AdministratorPermissionSet).GetProperties())
				{
					var oldVal = prop.GetValue(old);
					var newVal = prop.GetValue(newp);
					if ((oldVal == null && newVal == null) || oldVal.Equals(newVal))
					{
						continue;
					}
					var name = prop.GetName();
					sb.AppendLine($"{name} = {newVal}");
				}
				return sb.ToString();
			}
			if (context.User == null)
			{
				return false;
			}
			if (!IsAdmin(context.User, context, out var requesterPermissions) || !requesterPermissions.CanEditAdministrators)
			{
				return false;
			}
			if (!IsAdmin(target, context, out var existingPermissions))
			{
				return false;
			}
			if (existingPermissions == newPermissions)
			{
				return true;
			}
			var adminData = AdministratorData.GetValue(context.User.CreateRef(), context.Passphrase);
			var entry = adminData.Administrators.Single(ad => ad.User.Guid == target.Guid);
			if (entry == null)
			{
				return false;
			}
			entry.Permissions = newPermissions;
			AdministratorData.SetValue(adminData, context.User.CreateRef(), context.Passphrase);
			TokenCollection.AddOrUpdate(this, new EncryptedNotificationToken(context.User, null,
				$"Administrator @{context.User.Slug} altered @{target.Slug}'s administrator permissions:\n{GetPermissionDiff(existingPermissions, newPermissions)}",
				null, ENotificationType.Alert, EPermissionLevel.ADMIN, false));
			return true;
		}

		[View(EPermissionLevel.USER, customDrawer:"null")]
		public override Passphrase PublicKey => m_publicKey;
		[BsonElement]
		private Passphrase m_publicKey;

		public override Passphrase GetPrivateKey(AccessContext context)
		{
			if (context.User == null)
			{
				return default;
			}
			if (!IsAdmin(context.User, context, out _))
			{
				return default;
			}
			if (!AdministratorData.TryGetValue(context.User.CreateRef(), context.Passphrase, out var adminDataObj))
			{
				SecurityWatcher.RegisterEvent(context.User, $"User {context.User} tried to get Private Key, passed admin check but couldn't decrypt");
				throw new Exception("Bad auth");
			}
			return new Passphrase((adminDataObj as AdministrationData).GroupPrivateKey);
		}
		#endregion
	}
}
