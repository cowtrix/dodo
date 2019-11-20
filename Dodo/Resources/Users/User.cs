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

		public User() : base(null)
		{ }

		public User(WebPortalAuth auth, string password) : base(null)
		{
			WebAuth = auth;
		}

		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel visibility)
		{
			if(requestOwner == this)
			{
				visibility = EPermissionLevel.OWNER;
				return true;
			}
			visibility = EPermissionLevel.PUBLIC;
			return false;
		}
	}
}
