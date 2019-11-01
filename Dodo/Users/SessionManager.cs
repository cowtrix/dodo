using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.Users
{
	public class SessionManager
	{
		public class SessionManagerData
		{
			public ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>();
		}
		private SessionManagerData _data = new SessionManagerData();

		public User GetUserByUsername(string username)
		{
			return _data.Users.SingleOrDefault(x => x.Value.WebAuth.Username == username).Value;
		}

		public User CreateNewUser(WebPortalAuth webAuth)
		{
			webAuth.Validate();
			if (GetUserByUsername(webAuth.Username) != null)
			{
				throw new Exception("User already exists with username " + webAuth.Username);
			}
			var newUser = new User(webAuth);
			_data.Users[newUser.UUID] = newUser;
			return newUser;
		}

		public void DeleteUser(User user)
		{
			if(!_data.Users.TryRemove(user.UUID, out _))
			{
				throw new Exception("Failed to remove user " + user.UUID);
			}
		}
	}
}
