using Dodo;
using Dodo.Sites;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources.Security;

namespace Factory
{
	[TestClass]
	public class ActionSiteFactoryTests : FactoryTestBase<ActionSite, ActionSiteSchema>
	{
	}

	[TestClass]
	public class EventSiteFactoryTests : FactoryTestBase<EventSite, EventSiteSchema>
	{
	}

	[TestClass]
	public class SanctuarySiteFactoryTests : FactoryTestBase<SanctuarySite, SanctuarySiteSchema>
	{
	}

	[TestClass]
	public class OccupationSiteFactoryTests : FactoryTestBase<OccupationSite, OccupationSiteSchema>
	{
	}
}
