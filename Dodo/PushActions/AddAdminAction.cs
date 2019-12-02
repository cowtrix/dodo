using Newtonsoft.Json;
using SimpleHttpServer.REST;
using Common.Security;

namespace Dodo.Users
{
	public class AddAdminAction : PushAction
	{
		[JsonProperty]
		public ResourceReference<GroupResource> Resource { get; private set; }

		[JsonProperty]
		public byte[] Token { get; private set; }

		public override string Message => $"You have been added as an Administrator to {Resource.Value.Name}";

		public AddAdminAction(GroupResource resource, Passphrase temporaryPassword, string publicKey)
		{
			Resource = new ResourceReference<GroupResource>(resource);
			Token = AsymmetricSecurity.Encrypt(temporaryPassword.Value, publicKey);
		}

		public override void Execute(User user, Passphrase passphrase)
		{
			Resource.CheckValue();
			var privateKey = user.WebAuth.PrivateKey.GetValue(passphrase);
			var tempPass = new Passphrase(AsymmetricSecurity.Decrypt<string>(Token, privateKey));
			Resource.Value.AddAdmin(user, tempPass, user, passphrase);
		}
	}
}
