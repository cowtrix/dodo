using Common;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo.Users
{
	public class User : DodoResource
	{
		public class EncryptedData
		{
			public string PrivateString;
		}

		public const string ROOT = "u";
		public override string ResourceURL { get { return $"{ROOT}/{WebAuth.Username.StripForURL()}"; } }

		[NoPatch]
		[View(EViewVisibility.PUBLIC)]
		public WebPortalAuth WebAuth;
		[View(EViewVisibility.OWNER)]
		public string Email;
		[View(EViewVisibility.OWNER)]
		public EncryptedStore<EncryptedData> PrivateData;

		public User() : base(null)
		{ }

		public User(WebPortalAuth auth, string password) : base(null)
		{
			WebAuth = auth;
			PrivateData = new EncryptedStore<EncryptedData>(new EncryptedData(), password);
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
