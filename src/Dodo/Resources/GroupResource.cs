﻿using System;
using System.Collections.Generic;
using Common.Extensions;
using REST.Security;
using Dodo.Users;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using REST;
using REST.Serializers;
using Common.Security;

namespace Dodo
{
	public class GroupResourceReferenceSerializer : ResourceReferenceSerializer<GroupResource> { }

	public abstract class GroupResourceSchemaBase : DodoResourceSchemaBase 
	{
		public string PublicDescription { get; private set; }
		public GroupResource Parent { get; private set; }
		public GroupResourceSchemaBase(AccessContext context, string name, string publicDescription, GroupResource parent) 
			: base(context, name)
		{
			PublicDescription = publicDescription;
			Parent = parent;
		}
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

		public PushActionCollection SharedActions = new PushActionCollection();

		public SecureUserStore Members = new SecureUserStore();

		[View(EPermissionLevel.PUBLIC)]
		public int MemberCount { get { return Members.Count; } }

		[JsonProperty]
		public string GroupPublicKey { get; private set; }

		public GroupResource(GroupResourceSchemaBase schema) : base(schema)
		{
			Parent = new ResourceReference<GroupResource>(schema.Parent);
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			GroupPublicKey = pk;
			AdministratorData = new UserMultiSigStore<AdminData>(new AdminData(schema.Context.User, pv), schema.Context);
			PublicDescription = schema.PublicDescription;
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

		public bool IsAdmin(User target, AccessContext requesterContext)
		{
			var userRef = new ResourceReference<User>(requesterContext.User);
			if(!AdministratorData.IsAuthorised(userRef, requesterContext.Passphrase))
			{
				return false;
			}
			return AdministratorData.GetValue(userRef, requesterContext.Passphrase).Administrators.Contains(target);
		}

		public void AddAdmin(AccessContext context, User newAdmin, Passphrase newAdminPassword)
		{
			var userRef = new ResourceReference<User>(context.User);
			var newAdminRef = new ResourceReference<User>(newAdmin);
			AdministratorData.AddPermission(userRef, context.Passphrase, newAdminRef, newAdminPassword);
			var adminData = AdministratorData.GetValue(newAdminRef, newAdminPassword);
			var adminList = adminData.Administrators;
			if(adminList.Contains(newAdminRef))
			{
				return;
			}
			adminList.Add(newAdminRef);
			AdministratorData.SetValue(adminData, newAdminRef, newAdminPassword);
		}

		public override bool IsAuthorised(AccessContext context, EHTTPRequestType requestType, out EPermissionLevel permissionLevel)
		{
			if(context.User != null)
			{
				if (context.User.GUID == Creator.Value.GUID)
				{
					permissionLevel = EPermissionLevel.OWNER;
					return true;
				}
				if (IsAdmin(context.User, context))
				{
					permissionLevel = EPermissionLevel.ADMIN;
					return true;
				}
				if (requestType != EHTTPRequestType.GET)
				{
					permissionLevel = EPermissionLevel.PUBLIC;
					return false;
				}
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
			base.AppendAuxilaryData(view, permissionLevel, requester, passphrase);
		}
	}
}