using Common.Extensions;
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

		public User CreateTemporaryUser(string email, out Passphrase temporaryPassword)
		{
			temporaryPassword = new Passphrase(ValidationExtensions.GenerateStrongPassword());
			var schema = new UserRESTHandler.CreationSchema(Guid.NewGuid().ToString(), temporaryPassword.Value, "TEMPORARY", email);
			var newUser = new User(schema);
			using (var rscLock = new ResourceLock(newUser))
			{
				Add(newUser);
				newUser.PushActions.Add(new TemporaryUserAction(temporaryPassword, newUser.WebAuth.PublicKey));
				Update(newUser, rscLock);
				return newUser;
			}
		}
	}
}
