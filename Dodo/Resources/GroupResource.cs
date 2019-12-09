using System;
using System.Collections.Generic;
using Common;
using Common.Extensions;
using Common.Security;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo
{
	/// <summary>
	/// A group resource is either a Rebellion, Working Group or a Local Group
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

		[JsonProperty]
		[View(EPermissionLevel.PUBLIC)]
		public ResourceReference<GroupResource> Parent { get; private set; }

		[NoPatch]
		[View(EPermissionLevel.ADMIN)]
		public UserMultiSigStore<AdminData> AdministratorData;

		public PushActionCollection SharedActions = new PushActionCollection();

		public SecureUserStore Members = new SecureUserStore();

		[JsonProperty]
		public string GroupPublicKey { get; private set; }

		public GroupResource() : base() { }

		public GroupResource(User creator, Passphrase passphrase, string name, GroupResource parent) : base(creator, name)
		{
			Parent = new ResourceReference<GroupResource>(parent);
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			GroupPublicKey = pk;
			AdministratorData = new UserMultiSigStore<AdminData>(new AdminData(creator, pv), creator, passphrase);
		}

		/// <summary>
		/// Is this object a child of the target object
		/// </summary>
		/// <param name="targetObject"></param>
		/// <returns></returns>
		public bool IsChildOf(GroupResource targetObject)
		{
			if(Parent.Value == null)
			{
				return false;
			}
			if(Parent.Value == targetObject)
			{
				return true;
			}
			return Parent.Value.IsChildOf(targetObject);
		}

		public bool IsAdmin(User target, User requester, Passphrase passphrase)
		{
			var userRef = new ResourceReference<User>(requester);
			if(!AdministratorData.IsAuthorised(userRef, passphrase))
			{
				return false;
			}
			return AdministratorData.GetValue(userRef, passphrase).Administrators.Contains(target);
		}

		public void AddAdmin(User requester, Passphrase requesterPass, User newAdmin, Passphrase newAdminPassword)
		{
			var userRef = new ResourceReference<User>(requester);
			var newAdminRef = new ResourceReference<User>(newAdmin);
			AdministratorData.AddPermission(userRef, requesterPass, newAdminRef, newAdminPassword);
			var adminData = AdministratorData.GetValue(newAdminRef, newAdminPassword);
			var adminList = adminData.Administrators;
			if(adminList.Contains(newAdminRef))
			{
				return;
			}
			adminList.Add(newAdminRef);
			AdministratorData.SetValue(adminData, newAdminRef, newAdminPassword);
		}

		public void Join(User user, Passphrase passphrase)
		{
			Members.Add(user, passphrase);
		}

		public void Leave(User user, Passphrase passphrase)
		{
			Members.Remove(user, passphrase);
		}

		public override bool IsAuthorised(User requestOwner, Passphrase passphrase, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			if (requestOwner == Creator.Value)
			{
				permissionLevel = EPermissionLevel.OWNER;
				return true;
			}
			if (IsAdmin(requestOwner, requestOwner, passphrase))
			{
				permissionLevel = EPermissionLevel.ADMIN;
				return true;
			}
			if(request.Method != SimpleHttpServer.EHTTPRequestType.GET)
			{
				permissionLevel = EPermissionLevel.PUBLIC;
				return false;
			}
			if(requestOwner != null)
			{
				permissionLevel = EPermissionLevel.USER;
				return true;
			}
			else
			{
				permissionLevel = EPermissionLevel.PUBLIC;
				return true;
			}
		}

		public abstract bool CanContain(Type type);

		public override void AppendAuxilaryData(Dictionary<string, object> view, EPermissionLevel permissionLevel, 
			object requester, Passphrase passphrase)
		{
			var user = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).Value : requester as User;
			var isMember = Members.IsAuthorised(user, passphrase);
			view.Add(IS_MEMBER_AUX_TOKEN, isMember ? "true" : "false");
		}
	}
}
