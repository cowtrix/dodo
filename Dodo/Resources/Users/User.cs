using System;
using System.Collections.Generic;
using Common;
using Common.Extensions;
using Common.Security;
using Dodo.LocalGroups;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo.Users
{
	public class User : DodoResource
	{
		public const string ROOT = "u";
		public override string ResourceURL { get { return $"{ROOT}/{WebAuth.Username.StripForURL()}"; } }

		[NoPatch]
		[View(EPermissionLevel.OWNER)]
		public WebPortalAuth WebAuth;

		[View(EPermissionLevel.OWNER)]
		[Email]
		[NoPatch]
		public string Email;

		[View(EPermissionLevel.OWNER)]
		[UserFriendlyName]
		public string Name;

		[View(EPermissionLevel.OWNER)]
		public ResourceReference<LocalGroup> LocalGroup;

		[View(EPermissionLevel.OWNER)]
		public List<PushAction> PushActions = new List<PushAction>();

		public User() : base(null)
		{ }

		public User(WebPortalAuth auth) : base(null)
		{
			WebAuth = auth;
		}

		public override bool IsAuthorised(User requestOwner, string passphrase, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			if(requestOwner == this)
			{
				permissionLevel = EPermissionLevel.OWNER;
				return true;
			}
			permissionLevel = EPermissionLevel.PUBLIC;
			return false;
		}
	}
}
