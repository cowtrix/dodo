using Newtonsoft.Json;
using SimpleHttpServer.REST;
using Common.Security;

namespace Dodo.Users
{
	[SingletonPushAction]
	public class TemporaryUserAction : PushAction
	{
		[JsonProperty]
		public string TemporaryToken { get; private set; }

		public override string Message => "";

		public override bool AutoFire => true;

		public TemporaryUserAction(Passphrase temporaryPassword, string publicKey)
		{
			TemporaryToken = temporaryPassword.Value;
		}

	}
}
