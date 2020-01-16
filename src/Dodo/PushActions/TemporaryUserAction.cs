using Newtonsoft.Json;
using REST;
using REST.Security;

namespace Dodo.Users
{
	[SingletonPushAction]
	public class TemporaryUserAction : PushAction
	{
		[JsonProperty]
		public string TemporaryToken { get; private set; }

		public override bool AutoFire => true;

		public TemporaryUserAction(Passphrase temporaryPassword)
		{
			TemporaryToken = temporaryPassword.Value;
		}
	}
}
