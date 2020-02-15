using Dodo.Users;
using Resources;
using Resources.Security;
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
