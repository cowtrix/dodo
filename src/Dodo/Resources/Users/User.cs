using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Extensions;
using REST.Security;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.WorkingGroups;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using REST;
using REST.Serializers;

namespace Dodo.Users
{
	public class User : DodoResource
	{
		[ViewClass]
		public struct Notification
		{
			public string Message;
			public Guid GUID;
		}

		public const string ADMIN_OF_KEY = "ADMIN_OF";
		public const string ROLES_HELD_KEY = "ROLES";

		[View(EPermissionLevel.USER)]
		[VerifyObject]
		public WebPortalAuth WebAuth;

		[View(EPermissionLevel.OWNER)]
		[Email]
		[NoPatch]
		public string Email;

		[View(EPermissionLevel.ADMIN)]
		public bool EmailVerified { get; set; }

		[View(EPermissionLevel.OWNER)]
		public ResourceReference<LocalGroup> LocalGroup;

		public PushActionCollection PushActions = new PushActionCollection();

		[View(EPermissionLevel.OWNER)]
		public bool IsAdmin { get { return PushActions.GetSinglePushAction<AdminToken>() != null; } }

		[View(EPermissionLevel.OWNER)]
		public List<Notification> Notifications
		{
			get
			{
				return PushActions.Actions.Select(x => new Notification { Message = x.GetNotificationMessage(), GUID = x.GUID })
					.Where(x => !string.IsNullOrEmpty(x.Message)).ToList();
			}
		}

		public User(UserSchema schema) : base(schema)
		{
			WebAuth = new WebPortalAuth(schema.Username, schema.Password);
			Email = schema.Email;
		}

		public override bool IsAuthorised(AccessContext context, EHTTPRequestType requestType, out EPermissionLevel permissionLevel)
		{
			if(context.User.GUID == GUID)
			{
				permissionLevel = EPermissionLevel.OWNER;
				return true;
			}
			permissionLevel = EPermissionLevel.PUBLIC;
			return false;
		}

		public override void AppendAuxilaryData(Dictionary<string, object> view, EPermissionLevel permissionLevel, object requester, Passphrase passphrase)
		{
			var requesterUser = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).Value : (User)requester;
			if(permissionLevel >= EPermissionLevel.ADMIN)
			{
				var accessContext = new AccessContext(requesterUser, passphrase);
				view.Add(ADMIN_OF_KEY, new
				{
					Rebellions = ResourceUtility.GetManager<Rebellion>().Get(r => r.IsAdmin(this, accessContext)).Select(r => r.GUID),
					WorkingGroups = ResourceUtility.GetManager<WorkingGroup>().Get(r => r.IsAdmin(this, accessContext)).Select(r => r.GUID),
					LocalGroups = ResourceUtility.GetManager<LocalGroup>().Get(r => r.IsAdmin(this, accessContext)).Select(r => r.GUID),
				});
				view.Add(ROLES_HELD_KEY, ResourceUtility.GetManager<Role>().Get(r => r.RoleHolders.IsAuthorised(this, passphrase)).Select(r => r.GUID));
			}
			base.AppendAuxilaryData(view, permissionLevel, requester, passphrase);
		}
	}
}
