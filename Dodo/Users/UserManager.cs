using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.Users
{
	public class UserManager : ResourceManager<User>
	{
		public User CreateNew(WebPortalAuth webAuth)
		{
			webAuth.Validate();
			if (Get(x => x.WebAuth.Username == webAuth.Username) != null)
			{
				throw new Exception("User already exists with username " + webAuth.Username);
			}
			var newUser = new User(webAuth);
			InternalData.Entries[newUser.UUID] = newUser;
			return newUser;
		}
	}
}
