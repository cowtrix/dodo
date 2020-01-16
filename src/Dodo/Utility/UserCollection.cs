using Dodo.Users;
using REST;
using REST.Security;
using System.Collections.Generic;

namespace Dodo
{
	public class UserCollection : UserMultiSigStore<List<ResourceReference<User>>>
	{
		public UserCollection()
		{
		}

		public UserCollection(List<ResourceReference<User>> data, ResourceReference<User> key, Passphrase passphrase) : base(data, key, passphrase)
		{
		}

		public UserCollection(List<ResourceReference<User>> data, User key, Passphrase passphrase) : base(data, key, passphrase)
		{
		}

		public UserCollection(User key, Passphrase passphrase) : base(new List<ResourceReference<User>>() { key }, key, passphrase)
		{
		}
	}
}
