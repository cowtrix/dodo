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
		public string TemporaryToken { get; set; }
		[JsonIgnore]
		[BsonIgnore]
		public override bool Encrypted => false;

		public ResetPasswordToken() { }

		public ResetPasswordToken(User targetUser)
		{
			TemporaryToken = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
#if DEBUG
			Console.WriteLine($"Reset password action added for user {targetUser.AuthData.Username}: {TemporaryToken}");
#endif
		}

		public override void OnAdd(User user)
		{
			EmailHelper.SendEmail(user.PersonalData.Email, user.Name, $"{Dodo.PRODUCT_NAME}: Reset your password",
				$"You've requested a password reset for your account on {Dns.GetHostName()}." +
				$"To reset your password, visit the following link: {Dns.GetHostName()}/resetpassword?token={TemporaryToken}");
			base.OnAdd(user);
		}

		public string GetNotification(AccessContext context)
		{
			return "You've requested your password to be reset. " +
				"Check your email and click the link there. If this wasn't you, change your password immediately.";
		}
	}

}
