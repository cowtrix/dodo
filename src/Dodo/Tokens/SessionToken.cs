using Common.Config;
using Common.Security;
using Dodo.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using Resources.Security;
using System;
using System.Net;

namespace Dodo.Users.Tokens
{
	public class SessionToken : ExpiringToken
	{
		public static TimeSpan ShortSessionExpiryTime => ConfigManager.GetValue($"{nameof(SessionToken)}_{nameof(ShortSessionExpiryTime)}", TimeSpan.FromHours(1));
		public static TimeSpan LongSessionExpiryTime => ConfigManager.GetValue($"{nameof(SessionToken)}_{nameof(LongSessionExpiryTime)}", TimeSpan.FromDays(1));
		public const int KEYSIZE = 128;

		[JsonProperty]
		[BsonElement]
		public string UserKey { get; set; }
		[JsonProperty]
		[BsonElement]
		public string Address { get; set; }
		[JsonProperty]
		[BsonElement]
		public SymmEncryptedStore<string> EncryptedPassphrase { get; set; }
		[JsonIgnore]
		[BsonIgnore]
		public override bool Encrypted => false;

		[BsonConstructor]
		public SessionToken() { }

		public SessionToken(User user, string passphrase, Passphrase encryptionKey, IPAddress address, DateTime expiry) : base(expiry)
		{
			UserKey = KeyGenerator.GetUniqueKey(KEYSIZE);
#if DEBUG
			Address = new IPAddress(0).ToString();
#else
			Address = address == null ? new IPAddress(0).ToString() : address.ToString();
#endif
			EncryptedPassphrase = new SymmEncryptedStore<string>(passphrase, encryptionKey);
			SessionTokenStore.SetUser(UserKey, encryptionKey, user.Guid);
		}

		public override void OnRemove(AccessContext parent)
		{
			SessionTokenStore.RemoveUser(UserKey);
		}

		public override EPermissionLevel GetVisibility() => EPermissionLevel.PUBLIC;
	}
}
