using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo.Users.Tokens
{
	[SingletonToken]
	public class TemporaryUserToken : AutoExecutableToken
	{
		[JsonProperty]
		public string TemporaryToken { get; private set; }

		public TemporaryUserToken(Passphrase temporaryPassword)
		{
			TemporaryToken = temporaryPassword.Value;
		}
	}
}
