using Dodo;
using Dodo.Rebellions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REST.Security;

namespace Factory
{
	[TestClass]
	public class RebellionFactoryTests : FactoryTestBase<Rebellion, RebellionSchema>
	{
		protected override AccessContext GetCreationContext()
		{
			var user = GetRandomUser(out var password);

			return new AccessContext(user, 
				new Passphrase(user.WebAuth.PassPhrase.GetValue(password)));
		}

		protected override void VerifyCreatedObject(Rebellion obj, RebellionSchema schema)
		{
		}
	}
}
