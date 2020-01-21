using Dodo;
using Dodo.Sites;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REST.Security;

namespace Factory
{
	[TestClass]
	public class SiteFactoryTests : FactoryTestBase<Site, SiteSchema>
	{
		protected override AccessContext GetCreationContext()
		{
			var user = GetRandomUser(out var password);
			return new AccessContext(user,
				new Passphrase(user.WebAuth.PassPhrase.GetValue(password)));
		}

		protected override void VerifyCreatedObject(Site obj, SiteSchema schema)
		{
		}
	}
}
