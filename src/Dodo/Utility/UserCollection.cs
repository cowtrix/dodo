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

		public UserCollection(List<ResourceReference<User>> data, AccessContext context) : base(data, context)
		{
		}

		public UserCollection(AccessContext context) : base(new List<ResourceReference<User>>() { context.User }, context)
		{
		}
	}
}
