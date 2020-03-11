using Dodo;
using Dodo.Sites;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources.Security;

namespace Factory
{
	[TestClass]
	public class EventSiteFactoryTests : FactoryTestBase<EventSite, SiteSchema>
	{
	}

	[TestClass]
	public class SanctuarySiteFactoryTests : FactoryTestBase<MarchSite, SiteSchema>
	{
	}

	[TestClass]
	public class OccupationSiteFactoryTests : FactoryTestBase<PermanentSite, SiteSchema>
	{
	}
}
