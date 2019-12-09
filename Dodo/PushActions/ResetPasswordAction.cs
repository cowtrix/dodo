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
		public override string Message => "You've requested your password to be reset. " +
			"Check your email and click the link there. If this wasn't you, change your password immediately.";

		public override bool AutoFire => false;

		public ResourceReference<User> TargetUser;
		public Passphrase TemporaryToken;

		public ResetPasswordAction(User targetUser)
		{
			TargetUser = targetUser;
			TemporaryToken = new Passphrase(KeyGenerator.GetUniqueKey(TOKEN_SIZE), TimeSpan.FromMinutes(TOKEN_TIMEOUT));
			EmailHelper.SendEmail(targetUser.Email, targetUser.Name, $"{DodoServer.PRODUCT_NAME}: Reset your password",
				$"You've requested a password reset for your account on {DodoServer.GetURL()}." +
				$"To reset your password, visit the following link: {DodoServer.GetURL()}/resetpassword?token={TemporaryToken.Value}");

#if DEBUG
			Console.WriteLine(TemporaryToken.Value);
#endif
		}
	}

}
