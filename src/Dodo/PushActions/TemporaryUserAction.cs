using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo.Users
{
	[SingletonToken]
	public class TemporaryUserAction : UserToken
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
