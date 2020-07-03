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

namespace Dodo
{
	public interface IAdministratedResource : IRESTResource
	{
		bool IsAdmin(User target, AccessContext requesterContext, out AdministratorPermissionSet permissions);
		bool AddNewAdmin(AccessContext context, User newAdmin);
		bool UpdateAdmin(AccessContext context, User newAdmin, AdministratorPermissionSet permissions);
		bool CompleteAdminInvite(AccessContext context, User newAdmin, Passphrase newPass);
	}

	public class GroupResourceReferenceSerializer : ResourceReferenceSerializer<GroupResource> { }

	/// <summary>
	/// A group resource is either a Rebellion, Working Group or a Local Group.
	/// It can have administrators, which are authorised to edit it.
	/// It can have members and a public description.
	/// </summary>
	public abstract class GroupResource : 
		DodoResource, IPublicResource, ITokenResource, INotificationResource, IAdministratedResource
	{
		public const string IS_MEMBER_AUX_TOKEN = "isMember";
		/// <summary>
		/// This is a MarkDown formatted, public facing description of this resource
		/// </summary>
		[View(EPermissionLevel.PUBLIC, customDrawer: "markdown")]
		[Name("Public Description")]
		[Common.Extensions.Description]
		public string PublicDescription { get; set; }
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public UserMultiSigStore<AdministrationData> AdministratorData { get; set; }
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public int MemberCount { get { return Members.Count; } }
		[BsonElement]
		[View(EPermissionLevel.MEMBER, EPermissionLevel.SYSTEM, customDrawer:"null")]
		public string PublicKey { get; private set; }
		[Name("Published")]
		[View(EPermissionLevel.ADMIN, priority: -1, inputHint: IPublicResource.PublishInputHint)]
		public bool IsPublished { get; set; }

		public TokenCollection SharedTokens { get; set; } = new TokenCollection();

		public SecureUserStore Members { get; set; } = new SecureUserStore();

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
			SharedTokens.Add(this, new EncryptedNotificationToken(context.User, Name,
				$"Administrator @{context.User.Slug} added new Administrator @{newAdmin.Slug}",
				null, ENotificationType.Alert, false), EPermissionLevel.ADMIN);
			using (var userLock = new ResourceLock(newAdmin))
			{
				newAdmin.TokenCollection.Add(newAdmin, new UserAddedAsAdminToken(this, newPass, newAdmin.AuthData.PublicKey), EPermissionLevel.OWNER);
				ResourceUtility.GetManager<User>().Update(newAdmin, userLock);
			}
			return true;
		}

		public bool CompleteAdminInvite(AccessContext context, User newAdmin, Passphrase newPass)
		{
			if (newAdmin == null)
			{
				throw new ArgumentNullException(nameof(newAdmin));
			}
			if(!IsAdmin(context.User, context, out var administratorPermission))
			{
				// User has no pending invite
				return false;
			}
			if (newAdmin.Guid != context.User.Guid && IsAdmin(newAdmin, context, out _))
			{
				// Other user can't change existing admin password
				return true;
			}
			var newAdminRef = newAdmin.CreateRef();
			AdministratorData.AddPermission(context.User.CreateRef(), context.Passphrase, newAdminRef, newPass);
			var adminData = AdministratorData.GetValue(newAdminRef, newPass);
			if(!adminData.AddOrUpdateAdministrator(context, newAdmin))
			{
				return false;
			}
			AdministratorData.SetValue(adminData, newAdminRef, newPass);
#if DEBUG
			// Do a bit of extra testing just to make sure
			if (!IsAdmin(newAdmin, context, out _))
			{
				throw new Exception($"Failed to add {newAdmin} as a new administrator");
			}
#endif
			return true;
		}

		public bool UpdateAdmin(AccessContext context, User target, AdministratorPermissionSet permissions)
		{
			if(context.User == null)
			{
				return false;
			}
			if(!IsAdmin(context.User, context, out var requesterPermissions) || !requesterPermissions.CanChangePermissions)
			{
				return false;
			}
			if(!IsAdmin(target, context, out _))
			{
				return false;
			}
			var adminData = AdministratorData.GetValue(context.User.CreateRef(), context.Passphrase);
			var entry = adminData.Administrators.Single(ad => ad.User.Guid == target.Guid);
			if(entry == null)
			{
				return false;
			}
			entry.Permissions = permissions;
			AdministratorData.SetValue(adminData, context.User.CreateRef(), context.Passphrase);
			return true;
		}

		public abstract bool CanContain(Type type);

		/// <summary>
		/// We append several things to a group's metadata. 
		/// `isMember` - whether the requesting user is a member of the group.
		/// `notifications` - a collection of notifications that this user is allowed to view
		/// </summary>
		/// <param name="view"></param>
		/// <param name="permissionLevel"></param>
		/// <param name="requester"></param>
		/// <param name="passphrase"></param>
		public override void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel,
			object requester, Passphrase passphrase)
		{
			var user = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).GetValue() : requester as User;
			var isMember = Members.IsAuthorised(user.CreateRef(), passphrase);
			view.Add(IS_MEMBER_AUX_TOKEN, isMember ? "true" : "false");
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}

		public virtual void AddChild<T>(T rsc) where T : class, IOwnedResource
		{
			AddToken(new SimpleNotificationToken(null, Name, $"A new {rsc.GetType().GetName()} was created: \"{rsc.Name}\"", null, ENotificationType.Alert, false),
				EPermissionLevel.PUBLIC);
		}

		public virtual bool RemoveChild<T>(T rsc) where T : class, IOwnedResource
		{
			AddToken(new SimpleNotificationToken(null, Name, $"The {rsc.GetType().GetName()} \"{rsc.Name}\" was deleted.", null, ENotificationType.Alert, false), 
				EPermissionLevel.PUBLIC);
			return true;
		}

		public IEnumerable<Notification> GetNotifications(AccessContext context, EPermissionLevel permissionLevel)
		{
			Passphrase pass;
			return SharedTokens.GetNotifications(context, permissionLevel, this);
		}

		public void AddToken(IToken token, EPermissionLevel permissionLevel)
		{
			SharedTokens.Add(this, token, permissionLevel);
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
				throw new Exception($"User {context.User} tried to get Private Key but wasn't admin");
			}
			if (!AdministratorData.TryGetValue(context.User.CreateRef(), context.Passphrase, out var adminDataObj))
			{
				throw new Exception("Bad auth");
			}
			return new Passphrase((adminDataObj as AdministrationData).GroupPrivateKey);
		}
	}
}
