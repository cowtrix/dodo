using Dodo;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REST.Security;

namespace Factory
{
	[TestClass]
	public class WorkingGroupFactoryTests : FactoryTestBase<WorkingGroup, WorkingGroupSchema>
	{
		protected override AccessContext GetCreationContext()
		{
			var user = GetRandomUser(out var password);
			return new AccessContext(user,
				new Passphrase(user.WebAuth.PassPhrase.GetValue(password)));
		}

		protected override void VerifyCreatedObject(WorkingGroup obj, WorkingGroupSchema schema)
		{
		}
	}
}
