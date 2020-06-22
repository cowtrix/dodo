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

namespace Dodo
{
	public class GroupResourceReferenceSerializer : ResourceReferenceSerializer<GroupResource> { }

	/// <summary>
	/// A group resource is either a Rebellion, Working Group or a Local Group.
	/// It can have administrators, which are authorised to edit it.
	/// It can have members and a public description.
	/// </summary>
	public abstract class GroupResource : DodoResource, IPublicResource, ITokenResource, INotificationResource
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

		/// <summary>
		/// This is a MarkDown formatted, public facing description of this resource
		/// </summary>
		[View(EPermissionLevel.PUBLIC)]
		[Name("Public Description")]
		[Common.Extensions.Description]
		public string PublicDescription { get; set; }
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public UserMultiSigStore<AdminData> AdministratorData { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		public int MemberCount { get { return Members.Count; } }
		[View(EPermissionLevel.MEMBER)]
		[BsonElement]
		public string PublicKey { get; private set; }
		[View(EPermissionLevel.ADMIN)]
		public bool IsPublished { get; set; }

		public TokenCollection SharedTokens = new TokenCollection();

		public SecureUserStore Members = new SecureUserStore();

		public GroupResource() : base() { }

		public GroupResource(AccessContext context, DescribedResourceSchemaBase schema) : base(context, schema)
		{
			if(schema == null)
			{
				return;
			}
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			PublicKey = pk;
			AdministratorData = new UserMultiSigStore<AdminData>(new AdminData(context.User, pv), context);
			PublicDescription = schema.PublicDescription;
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
			SharedTokens.Add(this, new EncryptedNotificationToken(context.User, Name,
				$"Administrator @{context.User.Slug} added new Administrator @{newAdmin.Slug}",
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
			var isMember = Members.IsAuthorised(user, passphrase);
			view.Add(IS_MEMBER_AUX_TOKEN, isMember ? "true" : "false");
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}

		public virtual void AddChild<T>(T rsc) where T : class, IOwnedResource
		{
			AddToken(new SimpleNotificationToken(null, Name, $"A new {rsc.GetType().GetName()} was created: \"{rsc.Name}\"", false),
				EPermissionLevel.PUBLIC);
		}

		public virtual bool RemoveChild<T>(T rsc) where T : class, IOwnedResource
		{
			AddToken(new SimpleNotificationToken(null, Name, $"The {rsc.GetType().GetName()} \"{rsc.Name}\" was deleted.", false), 
				EPermissionLevel.PUBLIC);
			return true;
		}

		public IEnumerable<Notification> GetNotifications(AccessContext context, EPermissionLevel permissionLevel)
		{
			Passphrase pass;
			if (permissionLevel >= EPermissionLevel.ADMIN)
			{
				// Unlock encrypted administrator tokens
				pass = new Passphrase(AdministratorData.GetValue(context.User, context.Passphrase).GroupPrivateKey);
			}
			else
			{
				pass = context.Passphrase;
			}
			return SharedTokens.GetNotifications(context, pass, permissionLevel);
		}

		public void AddToken(IToken token, EPermissionLevel permissionLevel)
		{
			SharedTokens.Add(this, token, permissionLevel);
		}

		public bool DeleteNotification(AccessContext context, EPermissionLevel permissionLevel, Guid notification)
		{
			return SharedTokens.Remove(context, permissionLevel, notification);
		}
	}
}
