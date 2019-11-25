using Newtonsoft.Json;
using SimpleHttpServer.REST;
using Common;

namespace Dodo.Users
{
	public abstract class PushAction
	{
		public abstract void Execute(User user, string passphrase);
	}

	public class AddAdminAction : PushAction
	{
		[JsonProperty]
		public ResourceReference<GroupResource> Resource { get; private set; }

		[JsonProperty]
		public byte[] Token { get; private set; }

		public AddAdminAction(GroupResource resource, string temporaryPassword, string publicKey)
		{
			Resource = new ResourceReference<GroupResource>(resource);
			Token = AssymetricSecurity.Encrypt(temporaryPassword, publicKey);
		}

		public override void Execute(User user, string passphrase)
		{
			Resource.CheckValue();
			var privateKey = user.WebAuth.PrivateKey.GetValue(passphrase);
			var tempPass = AssymetricSecurity.Decrypt<string>(Token, privateKey);
			Resource.Value.AddAdmin(user, tempPass, user, passphrase);
		}
	}
}
