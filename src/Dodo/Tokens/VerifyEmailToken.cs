using Common.Extensions;
using Resources.Security;
using Newtonsoft.Json;
using System;
using Common.Security;
using MongoDB.Bson.Serialization.Attributes;
using Common;
using Resources;

namespace Dodo.Users.Tokens
{
	[SingletonToken]
	public class VerifyEmailToken : RedeemableToken, INotificationToken
	{
		public const int MAX_REQUEST_COUNT = 10;
		const int TOKEN_SIZE = 64;

		[JsonProperty]
		[BsonElement]
		public string Token { get; private set; }
		[JsonIgnore]
		[BsonIgnore]
		public override bool Encrypted => true;
		[JsonProperty]
		private Notification m_notification;
		[JsonProperty]
		public int ConfirmationEmailRequestCount { get; set; }

		public override void OnAdd(ITokenResource parent)
		{
			if(string.IsNullOrEmpty(Token))
			{
				Token = KeyGenerator.GetUniqueKey(TOKEN_SIZE).ToLowerInvariant();
				Logger.Info($"DEBUG: generated email verification key {Token} for user {parent}");
			}
			if(m_notification == null)
			{
				m_notification = new Notification(Guid, "Your Account", "You should check your email and verify your email address with us.", null, ENotificationType.Alert, EPermissionLevel.ADMIN);
			}
			base.OnAdd(parent);
		}

		public Notification GetNotification(AccessContext context) => m_notification;

		public override EPermissionLevel GetVisibility() => EPermissionLevel.ADMIN;
	}
}
