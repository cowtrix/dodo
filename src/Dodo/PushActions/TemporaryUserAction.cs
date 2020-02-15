using Newtonsoft.Json;
using Resources;
using Resources.Security;

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
