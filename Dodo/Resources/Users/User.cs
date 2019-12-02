using System;
using System.Collections.Generic;
using Common;
using Common.Extensions;
using Common.Security;
using Dodo.LocalGroups;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo.Users
{
	public class User : DodoResource
	{
		public const string ROOT = "u";
		public override string ResourceURL { get { return $"{ROOT}/{WebAuth.Username.StripForURL()}"; } }

		[View(EPermissionLevel.OWNER)]
		public WebPortalAuth WebAuth;

		[View(EPermissionLevel.OWNER)]
		[Email]
		[NoPatch]
		public string Email;

		[View(EPermissionLevel.OWNER)]
		public ResourceReference<LocalGroup> LocalGroup;

		[View(EPermissionLevel.OWNER)]
		public List<PushAction> PushActions = new List<PushAction>();

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
