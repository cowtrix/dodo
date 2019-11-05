using Common;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo.Users
{
	public class User : DodoResource
	{
		public override string ResourceURL { get { return $"u/{WebAuth.Username.StripForURL()}"; } }

		[NoPatch]
		[View(EViewVisibility.PUBLIC)]
		public WebPortalAuth WebAuth;
		[View(EViewVisibility.OWNER)]
		public string Email;

		public User() : base()
		{ }

		public User(WebPortalAuth auth) : base()
		{
			WebAuth = auth;
		}

		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EViewVisibility visibility)
		{
			throw new System.NotImplementedException();
		}
	}
}
