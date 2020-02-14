using Common.Extensions;
using REST.Security;
using Dodo.Resources;
using REST;
using System;

namespace Dodo.Users
{
	public class DodoUserManager : DodoResourceManager<User>
	{
		public User CreateTemporaryUser(string email, out Passphrase temporaryPassword)
		{
			temporaryPassword = new Passphrase(ValidationExtensions.GenerateStrongPassword());
			var schema = new UserSchema(Guid.NewGuid().ToString(), "TEMPORARY", temporaryPassword.Value, email);
			var newUser = new User(default, schema);
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
