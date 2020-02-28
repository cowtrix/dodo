using System;
using System.Collections.Generic;
using Common.Extensions;
using Resources.Security;
using Dodo.Users;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Resources;
using Resources.Serializers;
using Common.Security;

namespace Dodo
{
	public class GroupResourceReferenceSerializer : ResourceReferenceSerializer<GroupResource> { }

	public abstract class GroupResourceSchemaBase : DodoResourceSchemaBase 
	{
		public string PublicDescription { get; set; }
		public Guid Parent { get; set; }

		public GroupResourceSchemaBase(string name, string publicDescription, Guid parent)
			: base(name)
		{
			PublicDescription = publicDescription;
			Parent = parent;
		}

		public GroupResourceSchemaBase() : base() { }
	}

	/// <summary>
	/// A group resource is either a Rebellion, Working Group or a Local Group.
	/// It can have administrators, which are authorised to edit it.
	/// It can have members and a public description.
	/// </summary>
	public abstract class GroupResource : DodoResource
	{
		public const string IS_MEMBER_AUX_TOKEN = "IS_MEMBER";
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

		protected IResourceManager ResourceManager => ResourceUtility.GetManagerForResource(this);

		[JsonProperty]
		[View(EPermissionLevel.PUBLIC)]
		public ResourceReference<GroupResource> Parent { get; private set; }

		/// <summary>
		/// This is a MarkDown formatted, public facing description of this resource
		/// </summary>
		[View(EPermissionLevel.PUBLIC)]
		public string PublicDescription { get; set; }

		[NoPatch]
		[View(EPermissionLevel.ADMIN)]
		public UserMultiSigStore<AdminData> AdministratorData;

		public TokenCollection SharedActions = new TokenCollection();

		public SecureUserStore Members = new SecureUserStore();

		[View(EPermissionLevel.PUBLIC)]
		public int MemberCount { get { return Members.Count; } }

		[View(EPermissionLevel.MEMBER)]
		public string GroupPublicKey { get; private set; }

		[View(EPermissionLevel.ADMIN)]
		public bool Published { get; set; }

		public GroupResource(AccessContext context, GroupResourceSchemaBase schema) : base(context, schema)
		{
			if(schema == null)
			{
				return;
			}
			Parent = new ResourceReference<GroupResource>(schema.Parent);
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			GroupPublicKey = pk;
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
			return Parent.GetValue().GUID == targetObject.GUID;
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
			if(!AddOrUpdateAdmin(context, newAdmin, temporaryPass))
			{
				return false;
			}
			using (var userLock = new ResourceLock(newAdmin))
			{
				newAdmin.Tokens.Add(new AddAdminAction(this, temporaryPass, newAdmin.AuthData.PublicKey));
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
			if (IsAdmin(newAdmin, context))
			{
				return true;
			}
			using (var rscLock = new ResourceLock(this))
			{
				var group = rscLock.Value as GroupResource;
				group.AdministratorData.AddPermission(context.User, context.Passphrase, newAdmin, newPass);
				var adminData = group.AdministratorData.GetValue(context.User, context.Passphrase);
				var adminList = adminData.Administrators;
				if (adminList.Contains(newAdmin))
				{
					return true;
				}
				adminList.Add(newAdmin);
				group.AdministratorData.SetValue(adminData, newAdmin, newPass);
				ResourceUtility.GetManagerForResource(group).Update(group, rscLock);
				return true;
			}
		}

		public abstract bool CanContain(Type type);

		public override void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel,
			object requester, Passphrase passphrase)
		{
			var user = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).GetValue() : requester as User;
			var isMember = Members.IsAuthorised(user, passphrase);
			view.Add(IS_MEMBER_AUX_TOKEN, isMember ? "true" : "false");
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}
	}
}
