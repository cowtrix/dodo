using Dodo.Users;
using REST;
using REST.Security;
using System;

namespace Dodo
{
	public class UserMultiSigStore<T> : MultiSigEncryptedStore<ResourceReference<User>, T>
	{
		public UserMultiSigStore() : base() { }

		public UserMultiSigStore(T data, AccessContext context) : base(data, context.User, context.Passphrase)
		{
		}
	}
}
