using Common.Config;
using Common.Security;
using Dodo.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources.Security;
using System;

namespace Dodo.Users.Tokens
{
	public class SessionToken : ExpiringToken
	{
		public static TimeSpan SessionExpiryTime => ConfigManager.GetValue($"{nameof(SessionToken)}_{nameof(SessionExpiryTime)}", TimeSpan.FromDays(1));
		public const int KEYSIZE = 64;

		[JsonProperty]
		[BsonElement]
		public string UserKey { get; set; }
		[JsonProperty]
		[BsonElement]
		public SymmEncryptedStore<string> EncryptedPassphrase { get; set; }
		[JsonIgnore]
		[BsonIgnore]
		public override bool Encrypted => false;

		[BsonConstructor]
		public SessionToken() { }

		public SessionToken(User user, string passphrase, Passphrase encryptionKey) : base(DateTime.Now + SessionExpiryTime)
		{
			UserKey = KeyGenerator.GetUniqueKey(KEYSIZE);
			EncryptedPassphrase = new SymmEncryptedStore<string>(passphrase, encryptionKey);
			SessionTokenStore.SetUser(UserKey, encryptionKey, user.Guid);
		}

		public override void OnRemove(AccessContext parent)
		{
			SessionTokenStore.RemoveUser(UserKey);
		}
	}
}
