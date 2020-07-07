using Resources.Security;
using Resources;
using System;
using Common.Security;
using Dodo.Utility;
using System.Net;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;

namespace Dodo.Users.Tokens
{
	/// <summary>
	/// This token entitles the bearer to reset their password, if they provide the generated token.
	/// </summary>
	[SingletonToken]
	public class ResetPasswordToken : RedeemableToken, INotificationToken
	{
		const int TOKEN_SIZE = 64;
		public string Key { get; set; }
		[JsonIgnore]
		[BsonIgnore]
		public override bool Encrypted => false;
		[JsonProperty]
		private Notification m_notification;

		public ResetPasswordToken() { }

		public ResetPasswordToken(User targetUser)
		{
			Key = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
			m_notification = new Notification(Guid, "Your Account", "You've requested your password to be reset. " +
				"Check your email and click the link there. If this wasn't you, change your password immediately.",
				null, ENotificationType.Alert, EPermissionLevel.OWNER);
#if DEBUG
			Console.WriteLine($"Reset password action added for user {targetUser.Slug}: {Key}");
#endif
		}

		public Notification GetNotification(AccessContext context)
		{
			return m_notification;
		}

		public override EPermissionLevel GetVisibility() => EPermissionLevel.OWNER;
	}

}
