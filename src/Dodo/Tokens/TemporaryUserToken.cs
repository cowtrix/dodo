using Common.Extensions;
using Common.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo.Users.Tokens
{
	[SingletonToken]
	public class TemporaryUserToken : RedeemableToken
	{
		[JsonProperty]
		[BsonElement]
		public string Password { get; private set; }
		[JsonProperty]
		[BsonElement]
		public string TokenChallenge { get; private set; }
		[JsonIgnore]
		[BsonIgnore]
		public override bool Encrypted => false;

		public TemporaryUserToken() { }

		public TemporaryUserToken(Passphrase password, string tokenChallenge)
		{
			Password = password.Value;
			TokenChallenge = tokenChallenge;
		}

		public override EPermissionLevel GetVisibility() => EPermissionLevel.PUBLIC;
	}
}
