using Common.Extensions;
using Resources.Security;
using Dodo.Resources;
using Resources;
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
				newUser.Tokens.Add(new TemporaryUserAction(temporaryPassword));
				Update(newUser, rscLock);
				return newUser;
			}
		}
	}
}
