using Newtonsoft.Json;
using SimpleHttpServer.REST;
using Common.Security;

namespace Dodo.Users
{
	public class TemporaryUserAction : PushAction
	{
		[JsonProperty]
		public string TemporaryToken;

		public override string Message => "";

		public override bool AutoFire => false;

		public TemporaryUserAction(Passphrase temporaryPassword, string publicKey)
		{
			TemporaryToken = temporaryPassword.Value;
		}

		public override void Execute(User user, Passphrase passphrase)
		{
		}
	}
}
