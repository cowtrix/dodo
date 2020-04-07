using Common.Extensions;
using Common.Security;
using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo.Users.Tokens
{
	[SingletonToken]
	public class TemporaryUserToken : RedeemableToken
	{
		[JsonProperty]
		public string Password { get; private set; }
		[JsonProperty]
		public string TokenChallenge { get; private set; }

		public TemporaryUserToken(Passphrase password, string tokenChallenge)
		{
			Password = password.Value;
			TokenChallenge = tokenChallenge;
		}
	}
}
