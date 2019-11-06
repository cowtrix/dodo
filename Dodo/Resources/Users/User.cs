using Common;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo.Users
{
	public class User : DodoResource
	{
		public const string ROOT = "u";
		public override string ResourceURL { get { return $"{ROOT}/{WebAuth.Username.StripForURL()}"; } }

		[NoPatch]
		[View(EViewVisibility.PUBLIC)]
		public WebPortalAuth WebAuth;
		[View(EViewVisibility.OWNER)]
		public string Email;

		public User() : base(null)
		{ }

		public User(WebPortalAuth auth) : base(null)
		{
			WebAuth = auth;
		}

		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EViewVisibility visibility)
		{
			if(requestOwner == this)
			{
				visibility = EViewVisibility.OWNER;
				return true;
			}
			visibility = EViewVisibility.HIDDEN;
			return false;
		}
	}
}
