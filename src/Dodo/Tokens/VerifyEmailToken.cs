using Common.Extensions;
using Resources.Security;
using Newtonsoft.Json;
using System;
using Common.Security;
using MongoDB.Bson.Serialization.Attributes;

namespace Dodo.Users.Tokens
{
	[SingletonToken]
	public class VerifyEmailToken : RedeemableToken, INotificationToken
	{
		const int TOKEN_SIZE = 64;

		[JsonProperty]
		[BsonElement]
		public string Token { get; private set; }
		[JsonIgnore]
		[BsonIgnore]
		public override bool Encrypted => true;

		public override void OnAdd(User parent)
		{
			Token = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
			base.OnAdd(parent);
		}

		public string GetNotification(AccessContext context) => "You should check your email and verify your email address with us.";
	}
}
