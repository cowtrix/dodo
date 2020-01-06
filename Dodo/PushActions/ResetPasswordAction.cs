using Common.Security;
using Dodo.Utility;
using SimpleHttpServer.REST;
using System;

namespace Dodo.Users
{
	[SingletonPushAction]
	public class ResetPasswordAction : PushAction
	{
		const int TOKEN_SIZE = 32;
		const int TOKEN_TIMEOUT = 20;
		public ResourceReference<User> TargetUser;
		public string TemporaryToken;

		public ResetPasswordAction(User targetUser)
		{
			TargetUser = targetUser;
			TemporaryToken = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
#if DEBUG
			Console.WriteLine(TemporaryToken);
#endif
		}

		public override void OnAdd()
		{
			EmailHelper.SendEmail(TargetUser.Value.Email, TargetUser.Value.Name, $"{DodoServer.PRODUCT_NAME}: Reset your password",
				$"You've requested a password reset for your account on {DodoServer.GetURL()}." +
				$"To reset your password, visit the following link: {DodoServer.GetURL()}/resetpassword?token={TemporaryToken}");
		}

		public override string GetNotificationMessage()
		{
			return "You've requested your password to be reset. " +
				"Check your email and click the link there. If this wasn't you, change your password immediately.";
		}
	}

}
