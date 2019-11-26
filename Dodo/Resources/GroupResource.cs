using System;
using System.Collections.Generic;
using Common;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer.REST;

namespace Dodo
{
	/// <summary>
	/// A group resource is either a Rebellion, Working Group or a Local Group
	/// </summary>
	public abstract class GroupResource : DodoResource
	{
		[JsonProperty]
		public ResourceReference<GroupResource> Parent { get; private set; }

		[NoPatch]
		[View(EUserPriviligeLevel.ADMIN)]
		public UserMultiSigStore<List<ResourceReference<User>>> Administrators;

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

		public GroupResource(User creator, string passphrase, GroupResource parent) : base(creator)
		{
			Parent = new ResourceReference<GroupResource>(parent);
			Administrators = new UserMultiSigStore<List<ResourceReference<User>>>(
				new List<ResourceReference<User>>() { new ResourceReference<User>(creator) },
				creator, passphrase);

		}

		public bool IsAdmin(User user, string passphrase)
		{
			var userRef = new ResourceReference<User>(user);
			return Administrators.GetValue(userRef, passphrase).Contains(userRef);
		}

		public void AddAdmin(User requester, string requesterPass, User newAdmin, string newAdminPassword)
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

		public abstract bool CanContain(Type type);
	}
}
