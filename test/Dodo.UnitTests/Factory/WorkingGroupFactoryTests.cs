using Dodo;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REST.Security;

namespace Factory
{
	[TestClass]
	public class WorkingGroupFactoryTests : FactoryTestBase<WorkingGroup, WorkingGroupSchema>
	{
		protected override void VerifyCreatedObject(WorkingGroup obj, WorkingGroupSchema schema)
		{
		}
	}
}
