using Dodo;
using Dodo.Roles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REST.Security;

namespace Factory
{
	[TestClass]
	public class RoleFactoryTests : FactoryTestBase<Role, RoleSchema>
	{
		protected override AccessContext GetCreationContext()
		{
			var user = GetRandomUser(out var password);
			return new AccessContext(user,
				new Passphrase(user.WebAuth.PassPhrase.GetValue(password)));
		}

		protected override void VerifyCreatedObject(Role obj, RoleSchema schema)
		{
		}
	}
}
