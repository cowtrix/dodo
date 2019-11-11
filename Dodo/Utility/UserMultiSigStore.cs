using Common;
using Common.Security;
using Dodo.Users;
using SimpleHttpServer.REST;

namespace Dodo
{
	public class UserMultiSigStore<T> : MultiSigEncryptedStore<ResourceReference<User>, T>
	{
		public UserMultiSigStore(T data, ResourceReference<User> key, string password) : base(data, key, password)
		{
		}

		public UserMultiSigStore(T data, User key, string password) : base(data, new ResourceReference<User>(key), password)
		{
		}
	}
}
