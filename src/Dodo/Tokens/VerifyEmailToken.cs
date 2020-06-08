using Common.Extensions;
using Resources.Security;
using Newtonsoft.Json;
using System;
using Common.Security;
using MongoDB.Bson.Serialization.Attributes;
using Common;

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
		[JsonProperty]
		private Notification m_notification;

		public override void OnAdd(ITokenOwner parent)
		{
			Token = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
#if DEBUG
			Logger.Info($"DEBUG: generated email verification key {Token} for user {parent}");
#endif
			m_notification = new Notification("Your Account", "You should check your email and verify your email address with us.");
			base.OnAdd(parent);
		}

		public Notification GetNotification(AccessContext context) => m_notification;
	}
}
