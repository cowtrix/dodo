﻿using REST.Security;
using REST;
using System;
using Common.Security;
using Dodo.Utility;
using System.Net;

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
			Console.WriteLine($"Reset password action added for user {targetUser.WebAuth.Username}: {TemporaryToken}");
#endif
		}

		public override void OnAdd()
		{
			EmailHelper.SendEmail(TargetUser.Value.Email, TargetUser.Value.Name, $"{Dodo.PRODUCT_NAME}: Reset your password",
				$"You've requested a password reset for your account on {Dns.GetHostName()}." +
				$"To reset your password, visit the following link: {Dns.GetHostName()}/resetpassword?token={TemporaryToken}");
		}

		public override string GetNotificationMessage()
		{
			return "You've requested your password to be reset. " +
				"Check your email and click the link there. If this wasn't you, change your password immediately.";
		}
	}

}