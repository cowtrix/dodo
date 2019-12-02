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
		[JsonProperty]
		[View(EPermissionLevel.PUBLIC)]
		public ResourceReference<GroupResource> Parent { get; private set; }

		[NoPatch]
		[View(EPermissionLevel.ADMIN)]
		public UserMultiSigStore<List<ResourceReference<User>>> Administrators;

		public GroupResource() : base() { }

		public GroupResource(User creator, Passphrase passphrase, string name, GroupResource parent) : base(creator, name)
		{
			Parent = new ResourceReference<GroupResource>(parent);
			Administrators = new UserMultiSigStore<List<ResourceReference<User>>>(
				new List<ResourceReference<User>>() { new ResourceReference<User>(creator) },
				creator, passphrase);
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

		public bool IsAdmin(User user, Passphrase passphrase)
		{
			var userRef = new ResourceReference<User>(user);
			if(!Administrators.IsAuthorised(userRef, passphrase))
			{
				return false;
			}
			return Administrators.GetValue(userRef, passphrase).Contains(userRef);
		}

		public void AddAdmin(User requester, Passphrase requesterPass, User newAdmin, Passphrase newAdminPassword)
		{
			var userRef = new ResourceReference<User>(requester);
			var newAdminRef = new ResourceReference<User>(newAdmin);
			Administrators.AddPermission(userRef, requesterPass, newAdminRef, newAdminPassword);
			var adminList = Administrators.GetValue(newAdminRef, newAdminPassword);
			if(adminList.Contains(newAdminRef))
			{
				return;
			}
			adminList.Add(newAdminRef);
			Administrators.SetValue(adminList, newAdminRef, newAdminPassword);
		}

		public override bool IsAuthorised(User requestOwner, Passphrase passphrase, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			if (requestOwner == Creator.Value)
			{
				permissionLevel = EPermissionLevel.OWNER;
				return true;
			}
			if (IsAdmin(requestOwner, passphrase))
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
	}
}
