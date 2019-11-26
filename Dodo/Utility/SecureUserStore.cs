﻿using System.Collections.Generic;
using Common.Security;
using Dodo.Users;
using SimpleHttpServer.REST;

namespace Dodo
{
	public class SecureUserStore : MultiSigKeyStore<ResourceReference<User>>
	{
		public SecureUserStore(ResourceReference<User> key, string passphrase) : base(key, passphrase)
		{
		}
	}
}