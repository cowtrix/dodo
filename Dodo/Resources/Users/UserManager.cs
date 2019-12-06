﻿using Common.Extensions;
using Common.Security;
using Dodo.Resources;
using Dodo.Utility;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.Users
{
	public class UserManager : DodoResourceManager<User>
	{
		public override string BackupPath => "users";

		public User CreateTemporaryUser(string email, out Passphrase temporaryPassword)
		{
			temporaryPassword = new Passphrase(ValidationExtensions.GenerateStrongPassword());
			var schema = new UserRESTHandler.CreationSchema(Guid.NewGuid().ToString(), temporaryPassword.Value, "TEMPORARY", email);
			var newUser = new User(schema);
			Add(newUser);
			newUser.PushActions.Add(new TemporaryUserAction(temporaryPassword, newUser.WebAuth.PublicKey));
			return newUser;
		}

		public void SendEmailVerification(User user)
		{
			if(user.EmailVerified)
			{
				throw new HttpException("User email already verified", 200);
			}
			var emailVerifyPushAction = new VerifyEmailAction(user);
			user.PushActions.Add(emailVerifyPushAction);
			EmailHelper.SendEmail(user.Email, user.Name, $"{DodoServer.PRODUCT_NAME}: Please verify your email",
				"To verify your email, click the following link:\n" +
				$"{DodoServer.GetURL()}/{user.ResourceURL}?verify={emailVerifyPushAction.Token}");
		}
	}
}
