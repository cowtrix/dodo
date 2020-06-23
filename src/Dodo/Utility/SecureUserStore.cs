using System.Collections.Generic;
using Resources.Security;
using Dodo.Users;
using Resources;

namespace Dodo
{
	public class SecureUserStore : MultiSigKeyStore<ResourceReference<User>>
	{
		public bool IsAuthorised(AccessContext context)
		{
			return base.IsAuthorised(context.User.CreateRef(), context.Passphrase);
		}
	}
}
