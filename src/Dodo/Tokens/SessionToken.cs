using Common.Config;
using Common.Security;
using Dodo.Security;
using Newtonsoft.Json;
using Resources.Security;
using System;

namespace Dodo.Users.Tokens
{
	public class SessionToken : ExpiringToken
	{
		static TimeSpan SessionExpiryTime => ConfigManager.GetValue($"{nameof(SessionToken)}_{nameof(SessionExpiryTime)}", TimeSpan.FromDays(1));
		public const int KEYSIZE = 64;

		[JsonProperty]
		public string UserToken { get; private set; }
		[JsonProperty]
		public EncryptedStore<string> EncryptedPassphrase { get; private set; }

		public SessionToken() : base()
		{
		}

		public SessionToken(User user, string passphrase, Passphrase encryptionKey) : base(DateTime.Now + SessionExpiryTime)
		{
			UserToken = KeyGenerator.GetUniqueKey(KEYSIZE);
			EncryptedPassphrase = new EncryptedStore<string>(passphrase, encryptionKey);
			SessionTokenStore.SetUser(UserToken, encryptionKey, user.Guid);
		}

		public override void OnRemove(User parent)
		{
			SessionTokenStore.RemoveUser(UserToken);
		}
	}
}
