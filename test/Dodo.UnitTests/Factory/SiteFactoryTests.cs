using Dodo;
using Dodo.LocationResources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources.Security;

namespace Factory
{
	[TestClass]
	public class EventSiteFactoryTests : FactoryTestBase<Event, LocationResourceSchema>
	{
	}

	[TestClass]
	public class SiteFactoryTests : FactoryTestBase<Site, LocationResourceSchema>
	{
	}
}
