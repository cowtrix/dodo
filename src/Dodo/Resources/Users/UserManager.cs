using Common.Extensions;
using REST.Security;
using Dodo.Resources;
using REST;
using System;

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
				newUser.PushActions.Add(new TemporaryUserAction(temporaryPassword));
				Update(newUser, rscLock);
				return newUser;
			}
		}
	}
}
