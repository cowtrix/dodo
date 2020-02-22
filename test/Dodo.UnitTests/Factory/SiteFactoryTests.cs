using Dodo;
using Dodo.Sites;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources.Security;

namespace Factory
{
	[TestClass]
	public class ActionSiteFactoryTests : FactoryTestBase<ActionSite, SiteSchema>
	{
	}

	[TestClass]
	public class EventSiteFactoryTests : FactoryTestBase<EventSite, SiteSchema>
	{
	}

	[TestClass]
	public class SanctuarySiteFactoryTests : FactoryTestBase<SanctuarySite, SiteSchema>
	{
	}

	[TestClass]
	public class OccupationSiteFactoryTests : FactoryTestBase<OccupationSite, SiteSchema>
	{
	}
}
