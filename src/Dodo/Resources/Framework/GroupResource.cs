using System;
using System.Collections.Generic;
using Resources.Security;
using Dodo.Users;
using Resources;
using Resources.Serializers;
using Common.Security;
using Dodo.Users.Tokens;
using Common;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.Linq;
using System.Security;
using Microsoft.AspNetCore.Mvc.Formatters.Internal;
using System.Text;

namespace Dodo
{
	public interface IGroupResource
	{
		bool IsMember(AccessContext context);
		int MemberCount { get; }
		void Join(AccessContext context);
		void Leave(AccessContext context);
	}

	public class GroupResourceReferenceSerializer : ResourceReferenceSerializer<GroupResource> { }

	/// <summary>
	/// A group resource is either a Rebellion, Working Group or a Local Group.
	/// It can have administrators, which are authorised to edit it.
	/// It can have members and a public description.
	/// </summary>
	public abstract class GroupResource : 
		DodoResource, IPublicResource, INotificationResource, IAdministratedResource, IGroupResource
	{
		public const string IS_MEMBER_AUX_TOKEN = "isMember";

		/// <summary>
		/// This is a MarkDown formatted, public facing description of this resource
		/// </summary>
		[View(EPermissionLevel.PUBLIC, customDrawer: "markdown")]
		[Name("Public Description")]
		[Resources.Description]
		public string PublicDescription { get; set; }

		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public UserMultiSigStore<AdministrationData> AdministratorData { get; set; }

		[BsonElement]
		[View(EPermissionLevel.MEMBER, EPermissionLevel.SYSTEM, customDrawer:"null")]
		public string PublicKey { get; private set; }

		[Name("Published")]
		[View(EPermissionLevel.ADMIN, priority: -1, inputHint: IPublicResource.PublishInputHint)]
		public bool IsPublished { get; set; }

		[BsonElement]
		private TokenCollection SharedTokens { get; set; } = new TokenCollection();

		[BsonElement]
		private SecureUserStore Members { get; set; } = new SecureUserStore();

		public GroupResource() : base() { }

		public GroupResource(AccessContext context, DescribedResourceSchemaBase schema) : base(context, schema)
		{
			if(schema == null)
			{
				return;
			}
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			PublicKey = pk;
			AdministratorData = new UserMultiSigStore<AdministrationData>(
				new AdministrationData(this, context.User, pv), context);
			PublicDescription = schema.PublicDescription;
		}

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
			if(entry == null)
			{
				return false;
			}
			permissions = entry.Permissions;
			return true;
		}

		public bool RemoveAdmin(AccessContext context, User targetUser)
		{
			if (!IsAdmin(context.User, context, out var administratorPermission) || !administratorPermission.CanRemoveAdmin)
			{
				// Context isn't admin, or doesn't have correct permissions
				SecurityWatcher.RegisterEvent(context.User, $"User {context.User} tried to remove {targetUser} as a new administrator for {this}, but they weren't an administrator.");
				return false;
			}
			var adminData = AdministratorData.GetValue(context.User.CreateRef(), context.Passphrase);
			adminData.Administrators = adminData.Administrators.Where(ad => ad.User.Guid != targetUser.Guid).ToList();
			AdministratorData.SetValue(adminData, context.User.CreateRef(), context.Passphrase);
			SharedTokens.Add(this, new EncryptedNotificationToken(context.User, null, $"Administrator @{context.User.Slug} removed @{targetUser.Slug} as an administrator",
				null, ENotificationType.Alert, EPermissionLevel.ADMIN, false));
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
			if (!IsAdmin(context.User, context, out var administratorPermission) || !administratorPermission.CanAddAdmin)
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
			SharedTokens.Add(this, new EncryptedNotificationToken(context.User, null,
				$"Administrator @{context.User.Slug} added new Administrator @{newAdmin.Slug}",
				null, ENotificationType.Alert, EPermissionLevel.ADMIN, false));
			using (var userLock = new ResourceLock(newAdmin))
			{
				newAdmin.TokenCollection.Add(newAdmin, new UserAddedAsAdminToken(this, newPass, newAdmin.AuthData.PublicKey));
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
			if(IsAdmin(context.User, context, out _))
			{
				// User has no pending invite
				return false;
			}
			var adminRef = context.User.CreateRef();
			AdministratorData.AddPermission(adminRef, tempPass, adminRef, context.Passphrase);
			var adminData = AdministratorData.GetValue(adminRef, context.Passphrase);
			if(!adminData.AddOrUpdateAdministrator(context, context.User))
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
				foreach(var prop in typeof(AdministratorPermissionSet).GetProperties())
				{
					var oldVal = prop.GetValue(old);
					var newVal = prop.GetValue(newp);
					if((oldVal == null && newVal == null) || oldVal.Equals(newVal))
					{
						continue;
					}
					var name = prop.GetName();
					sb.AppendLine($"{name} = {newVal}");
				}
				return sb.ToString();
			}
			if(context.User == null)
			{
				return false;
			}
			if(!IsAdmin(context.User, context, out var requesterPermissions) || !requesterPermissions.CanChangePermissions)
			{
				return false;
			}
			if(!IsAdmin(target, context, out var existingPermissions))
			{
				return false;
			}
			if(existingPermissions == newPermissions)
			{
				return true;
			}
			var adminData = AdministratorData.GetValue(context.User.CreateRef(), context.Passphrase);
			var entry = adminData.Administrators.Single(ad => ad.User.Guid == target.Guid);
			if(entry == null)
			{
				return false;
			}
			entry.Permissions = newPermissions;
			AdministratorData.SetValue(adminData, context.User.CreateRef(), context.Passphrase);
			SharedTokens.Add(this, new EncryptedNotificationToken(context.User, null,
				$"Administrator @{context.User.Slug} altered @{target.Slug}'s administrator permissions:\n{GetPermissionDiff(existingPermissions, newPermissions)}",
				null, ENotificationType.Alert, EPermissionLevel.ADMIN, false));
			return true;
		}
		#endregion

		#region Child Objects
		public abstract bool CanContain(Type type);

		public virtual void AddChild<T>(T rsc) where T : class, IOwnedResource
		{
			AddToken(new SimpleNotificationToken(null, null, $"A new {rsc.GetType().GetName()} was created: \"{rsc.Name}\"", 
				$"{Dodo.DodoApp.NetConfig.FullURI}/{rsc.GetType().Name.ToLowerInvariant()}/{rsc.Slug}", ENotificationType.Alert, EPermissionLevel.ADMIN, false));
		}

		public virtual bool RemoveChild<T>(T rsc) where T : class, IOwnedResource
		{
			AddToken(new SimpleNotificationToken(null, null, $"The {rsc.GetType().GetName()} \"{rsc.Name}\" was deleted.", 
				null, ENotificationType.Alert, EPermissionLevel.ADMIN, false));
			return true;
		}
		#endregion

		#region Notifications & Tokens
		public IEnumerable<Notification> GetNotifications(AccessContext context, EPermissionLevel permissionLevel)
		{
			return SharedTokens.GetNotifications(context, permissionLevel, this);
		}

		public void AddToken(IToken token)
		{
			SharedTokens.Add(this, token);
		}

		public bool DeleteNotification(AccessContext context, EPermissionLevel permissionLevel, Guid notification)
		{
			return SharedTokens.Remove(context, permissionLevel, notification, this);
		}

		public Passphrase GetPrivateKey(AccessContext context)
		{
			if (context.User == null)
			{
				return default;
			}
			if(!IsAdmin(context.User, context, out _))
			{
				SecurityWatcher.RegisterEvent(context.User, $"User {context.User} tried to get Private Key but wasn't admin");
				throw new Exception("Bad auth");
			}
			if (!AdministratorData.TryGetValue(context.User.CreateRef(), context.Passphrase, out var adminDataObj))
			{
				SecurityWatcher.RegisterEvent(context.User, $"User {context.User} tried to get Private Key, passed admin check but couldn't decrypt");
				throw new Exception("Bad auth");
			}
			return new Passphrase((adminDataObj as AdministrationData).GroupPrivateKey);
		}
		#endregion

		#region Group
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public int MemberCount { get { return Members.Count; } }

		public bool IsMember(AccessContext context)
		{
			if (!context.Challenge())
			{
				return false;
			}
			return Members.IsAuthorised(context);
		}

		public void Leave(AccessContext accessContext)
		{
			Members.Remove(accessContext.User.CreateRef(), accessContext.Passphrase);
		}

		public void Join(AccessContext accessContext)
		{
			Members.Add(accessContext.User.CreateRef(), accessContext.Passphrase);
		}
		#endregion

		public override void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel,
			object requester, Passphrase passphrase)
		{
			var user = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).GetValue() : requester as User;
			var context = new AccessContext(user, passphrase);
			view.Add(IS_MEMBER_AUX_TOKEN, IsMember(context) ? "true" : "false");
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}
	}
}
