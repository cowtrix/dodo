using System;
using System.Collections.Generic;
using Common;
using Common.Extensions;
using Common.Security;
using Dodo.LocalGroups;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using SimpleHttpServer.REST.Serializers;

namespace Dodo.Users
{
	public class UserSerializer : ResourceReferenceSerializer<User> { }

	public class User : DodoResource
	{
		public const string ROOT = "u";
		public override string ResourceURL { get { return $"{ROOT}/{WebAuth.Username.StripForURL()}"; } }

		[View(EPermissionLevel.OWNER)]
		[VerifyObject]
		public WebPortalAuth WebAuth;

		[View(EPermissionLevel.OWNER)]
		[Email]
		[NoPatch]
		public string Email;

		[View(EPermissionLevel.OWNER)]
		public bool EmailVerified { get; set; }

		[View(EPermissionLevel.OWNER)]
		public ResourceReference<LocalGroup> LocalGroup;

		[View(EPermissionLevel.OWNER)]
		public PushActionCollection PushActions = new PushActionCollection();

		public bool IsAdmin { get { return PushActions.GetSinglePushAction<AdminToken>() != null; } }

		public User() : base()
		{
		}

		public User(UserRESTHandler.CreationSchema info) : base(null, info.Name)
		{
			WebAuth = new WebPortalAuth(info.Username, info.Password);
			Email = info.Email;
		}

		public override bool IsAuthorised(User requestOwner, Passphrase passphrase, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			if(requestOwner.GUID == GUID)
			{
				permissionLevel = EPermissionLevel.OWNER;
				return true;
			}
			permissionLevel = EPermissionLevel.PUBLIC;
			return false;
		}
	}
}
