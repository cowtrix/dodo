using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo.Users
{
	[SingletonToken]
	public class TemporaryUserAction : OneTimeRedeemableToken
	{
		[JsonProperty]
		public string TemporaryToken { get; private set; }

		public TemporaryUserAction(Passphrase temporaryPassword)
		{
			TemporaryToken = temporaryPassword.Value;
		}
	}
}
