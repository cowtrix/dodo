using Common;
using Common.Security;
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
		[View(EPermissionLevel.USER)]
		public WebPortalAuth WebAuth;
		[View(EPermissionLevel.OWNER)]
		public string Email;
		[View(EPermissionLevel.OWNER)]
		public EncryptedStore<EncryptedData> PrivateData;

		public User() : base(null)
		{ }

		public User(WebPortalAuth auth, string password) : base(null)
		{
			WebAuth = auth;
			PrivateData = new EncryptedStore<EncryptedData>(new EncryptedData(), password);
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
