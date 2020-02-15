using System.Collections.Generic;
using Resources.Security;
using Dodo.Users;
using Resources;

namespace Dodo
{
	public class SecureUserStore : MultiSigKeyStore<ResourceReference<User>>
	{
	}
}
