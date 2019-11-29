using Common;
using Common.Security;
using Dodo.Users;
using SimpleHttpServer.REST;

namespace Dodo
{
	public class UserMultiSigStore<T> : MultiSigEncryptedStore<ResourceReference<User>, T>
	{
		public UserMultiSigStore(T data, ResourceReference<User> key, Passphrase passphrase) : base(data, key, passphrase)
		{
		}

		public UserMultiSigStore(T data, User key, Passphrase passphrase) : base(data, new ResourceReference<User>(key), passphrase)
		{
		}
	}
}
