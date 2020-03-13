using Common.Security;
using Dodo.Security;
using Newtonsoft.Json;
using Resources.Security;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Dodo.Users.Tokens
{
	public class SessionToken : UserToken
	{
		public const int KEYSIZE = 64;

		[JsonProperty]
		public string UserToken { get; private set; }
		[JsonProperty]
		public EncryptedStore<string> EncryptedPassphrase { get; private set; }

		public SessionToken(User user, string passphrase, Passphrase encryptionKey)
		{
			UserToken = KeyGenerator.GetUniqueKey(KEYSIZE);
			EncryptedPassphrase = new EncryptedStore<string>(passphrase, encryptionKey);
			SessionTokenStore.SetUser(UserToken, encryptionKey, user.GUID);
		}
	}
}
