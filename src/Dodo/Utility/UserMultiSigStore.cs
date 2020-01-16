using Dodo.Users;
using REST;
using REST.Security;

namespace Dodo
{
	public class UserMultiSigStore<T> : MultiSigEncryptedStore<ResourceReference<User>, T>
	{
		public UserMultiSigStore() : base() { }

		public UserMultiSigStore(T data, ResourceReference<User> key, Passphrase passphrase) : base(data, key, passphrase)
		{
		}

		public UserMultiSigStore(T data, User key, Passphrase passphrase) : base(data, key, passphrase)
		{
		}
	}
}
