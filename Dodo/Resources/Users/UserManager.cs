using Common.Extensions;
using Dodo.Resources;
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

		public User CreateTemporaryUser(string email)
		{
			var password = ValidationExtensions.GenerateStrongPassword();
			var schema = new UserRESTHandler.CreationSchema(Guid.NewGuid().ToString(), password, "TEMPORARY", email);

			var newUser = new User(schema);
			Add(newUser);

			newUser.PushActions.Add(new TemporaryUserAction(password, newUser.WebAuth.PublicKey));

			return newUser;
		}
	}
}
