using System;
using System.Collections.Generic;
using Common.Extensions;
using Resources.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using Resources.Serializers;
using Common.Security;
using Dodo.Users.Tokens;
using Dodo.Resources;
using Common;
using MongoDB.Bson.Serialization.Attributes;

namespace Dodo
{
	public class GroupResourceReferenceSerializer : ResourceReferenceSerializer<GroupResource> { }

	public abstract class OwnedResourceSchemaBase : ResourceSchemaBase 
	{
		public string PublicDescription { get; set; }
		public Guid Parent { get; set; }

		public OwnedResourceSchemaBase(string name, string publicDescription, Guid parent)
			: base(name)
		{
			PublicDescription = publicDescription;
			Parent = parent;
		}

		public OwnedResourceSchemaBase() : base() { }
	}

	/// <summary>
	/// A group resource is either a Rebellion, Working Group or a Local Group.
	/// It can have administrators, which are authorised to edit it.
	/// It can have members and a public description.
	/// </summary>
	public abstract class GroupResource : DodoResource, 
		IOwnedResource, IPublicResource, ITokenOwner
	{
		public const string IS_MEMBER_AUX_TOKEN = "isMember";
		public class AdminData
		{
			[View(EPermissionLevel.ADMIN)]
			public List<ResourceReference<User>> Administrators = new List<ResourceReference<User>>();
			public string GroupPrivateKey { get; private set; }
			public AdminData(User firstAdmin, string privateKey)
			{
				Administrators.Add(firstAdmin);
				GroupPrivateKey = privateKey;
			}
		}

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public ResourceReference<GroupResource> Parent { get; private set; }
		/// <summary>
		/// This is a MarkDown formatted, public facing description of this resource
		/// </summary>
		[View(EPermissionLevel.PUBLIC)]
		public string PublicDescription { get; set; }
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public UserMultiSigStore<AdminData> AdministratorData;
		[View(EPermissionLevel.PUBLIC)]
		public int MemberCount { get { return Members.Count; } }
		[View(EPermissionLevel.MEMBER)]
		[BsonElement]
		public string PublicKey { get; private set; }
		[View(EPermissionLevel.ADMIN)]
		public bool IsPublished { get; set; }

		public TokenCollection SharedTokens = new TokenCollection();

		public SecureUserStore Members = new SecureUserStore();

		public GroupResource(AccessContext context, OwnedResourceSchemaBase schema) : base(context, schema)
		{
			if(schema == null)
			{
				return;
			}
			var group = ResourceUtility.GetResourceByGuid<GroupResource>(schema.Parent);
			Parent = new ResourceReference<GroupResource>(group);
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			PublicKey = pk;
			AdministratorData = new UserMultiSigStore<AdminData>(new AdminData(context.User, pv), context);
			PublicDescription = schema.PublicDescription;
		}

		/// <summary>
		/// Is this object a child of the target object
		/// </summary>
		/// <param name="targetObject"></param>
		/// <returns></returns>
		public bool IsChildOf(GroupResource targetObject)
		{
			if(!Parent.HasValue())
			{
				return false;
			}
			return Parent.Guid == targetObject.Guid;
		}

		public bool IsAdmin(User target, AccessContext requesterContext)
		{
			var userRef = new ResourceReference<User>(requesterContext.User);
			if(!AdministratorData.IsAuthorised(userRef, requesterContext.Passphrase))
			{
				return false;
			}
			return AdministratorData.GetValue(userRef, requesterContext.Passphrase).Administrators.Contains(target);
		}

		public bool AddAdmin(AccessContext context, User newAdmin)
		{
			var temporaryPass = new Passphrase(KeyGenerator.GetUniqueKey(32));
			using (var groupLock = new ResourceLock(this))
			{
				if (!AddOrUpdateAdmin(context, newAdmin, temporaryPass))
				{
					return false;
				}
				ResourceManager.Update(this, groupLock);
			}
			using (var userLock = new ResourceLock(newAdmin))
			{
				newAdmin.TokenCollection.Add(newAdmin, new UserAddedAsAdminToken(this, temporaryPass, newAdmin.AuthData.PublicKey), EPermissionLevel.OWNER);
				ResourceUtility.GetManager<User>().Update(newAdmin, userLock);
			}
			return true;
		}

		public bool AddOrUpdateAdmin(AccessContext context, User newAdmin, Passphrase newPass)
		{
			if (newAdmin == null)
			{
				return false;
			}
			if (newAdmin.Guid != context.User.Guid && IsAdmin(newAdmin, context))
			{
				// Other user can't change existing admin password
				return true;
			}
			AdministratorData.AddPermission(context.User, context.Passphrase, newAdmin, newPass);
			var adminData = AdministratorData.GetValue(newAdmin, newPass);
			var adminList = adminData.Administrators;
			if (adminList.Contains(newAdmin))
			{
				return true;
			}
			adminList.Add(newAdmin);
			AdministratorData.SetValue(adminData, newAdmin, newPass);
			SharedTokens.Add(this, new EncryptedNotificationToken(Name,
				$"Administrator @{context.User.AuthData.Username} added new Administrator @{newAdmin.AuthData.Username}",
				false), EPermissionLevel.ADMIN);
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
			var context = new AccessContext(user, passphrase);
			var isMember = Members.IsAuthorised(user, passphrase);
			view.Add(IS_MEMBER_AUX_TOKEN, isMember ? "true" : "false");
			Passphrase pass;
			if(permissionLevel >= EPermissionLevel.ADMIN)
			{
				// Unlock encrypted administrator tokens
				pass = new Passphrase(AdministratorData.GetValue(user, passphrase).GroupPrivateKey);
			}
			else
			{
				pass = passphrase;
			}
			var notifications = SharedTokens.GetNotifications(context, pass, permissionLevel);
			view.Add(METADATA_NOTIFICATIONS_KEY, notifications);
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}

		public virtual void AddChild<T>(T rsc) where T : class, IOwnedResource
		{
			SharedTokens.Add(this, new SimpleNotificationToken(Name,
				$"A new {rsc.GetType().GetName()} was created: \"{rsc.Name}\"", true));
		}

		public virtual bool RemoveChild<T>(T rsc) where T : class, IOwnedResource
		{
			SharedTokens.Add(this, new SimpleNotificationToken(Name,
				$"The {rsc.GetType().GetName()} \"{rsc.Name}\" was deleted.", true));
			return true;
		}
	}
}
