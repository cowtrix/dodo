using Dodo;
using Dodo.Rebellions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources.Security;

namespace Factory
{
	[TestClass]
	public class RebellionFactoryTests : FactoryTestBase<Rebellion, RebellionSchema>
	{
		protected override void VerifyCreatedObject(Rebellion obj, RebellionSchema schema)
		{
		}
	}
}
