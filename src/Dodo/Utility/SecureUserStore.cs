using System.Collections.Generic;
using REST.Security;
using Dodo.Users;
using REST;

namespace Dodo
{
	public class SecureUserStore : MultiSigKeyStore<ResourceReference<User>>
	{
	}
}
