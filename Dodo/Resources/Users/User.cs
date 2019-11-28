using System;
using System.Collections.Generic;
using Common;
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
		[View(EUserPriviligeLevel.OWNER)]
		public WebPortalAuth WebAuth;

		[View(EUserPriviligeLevel.OWNER)]
		[Email]
		[NoPatch]
		public string Email;

		[View(EUserPriviligeLevel.OWNER)]
		[UserFriendlyName]
		public string Name;

		[View(EUserPriviligeLevel.OWNER)]
		public ResourceReference<LocalGroup> LocalGroup;

		public List<PushAction> PushActions = new List<PushAction>();

		public User() : base(null)
		{ }

		public User(WebPortalAuth auth) : base(null)
		{
			WebAuth = auth;
		}

		public override bool IsAuthorised(User requestOwner, string passphrase, HttpRequest request, out EUserPriviligeLevel permissionLevel)
		{
			if(requestOwner == this)
			{
				permissionLevel = EUserPriviligeLevel.OWNER;
				return true;
			}
			permissionLevel = EUserPriviligeLevel.PUBLIC;
			return false;
		}
	}
}
