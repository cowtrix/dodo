using Dodo;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.SharedTest;
using Dodo.Sites;
using DodoResources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using SharedTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilterTests
{
	[TestClass]
	public class RebellionDateFilterTests : DateFilterTests<Rebellion> { }

	[TestClass]
	public class ActionSiteDateFilterTests : DateFilterTests<ActionSite> { }

	[TestClass]
	public class EventSiteDateFilterTests : DateFilterTests<EventSite> { }

	[TestClass]
	public class MarchSiteDateFilterTests : DateFilterTests<MarchSite> { }

	public abstract class DateFilterTests<T> : TestBase where T: GroupResource, ITimeBoundResource
	{
		[TestMethod]
		public void FilterByDate()
		{
			if(typeof(T).IsAssignableFrom(typeof(ITimeBoundResource)))
			{
				Assert.Inconclusive();
			}
			var user = GetRandomUser(out var password, out var context);
			var schema = SchemaGenerator.GetRandomSchema<T>(context);
			var factory = ResourceUtility.GetFactory<T>();
			var rsc = factory.CreateTypedObject(context, schema);
			var filter = new DateFilter() { startdate = rsc.StartDate.ToShortDateString() };
			var manager = ResourceUtility.GetManager(typeof(T));
			var search = manager.Get(x => filter.Filter(x));
			Assert.IsTrue(search.Single().GUID == rsc.GUID);
		}
	}

	[TestClass]
	public class RebellionLocationFilterTests : LocationFilterTests<Rebellion> { }

	[TestClass]
	public class ActionSiteLocationFilterTests : LocationFilterTests<ActionSite> { }

	[TestClass]
	public class EventSiteLocationFilterTests : LocationFilterTests<EventSite> { }

	[TestClass]
	public class MarchSiteLocationFilterTests : LocationFilterTests<MarchSite> { }

	[TestClass]
	public class SanctuarySiteLocationFilterTests : LocationFilterTests<SanctuarySite> { }

	[TestClass]
	public class LocalGroupLocationFilterTests : LocationFilterTests<LocalGroup> { }

	public abstract class LocationFilterTests<T> : TestBase where T : GroupResource, ILocationalResource
	{
		[TestMethod]
		public void FilterByLocation()
		{
			if (typeof(T).IsAssignableFrom(typeof(ITimeBoundResource)))
			{
				Assert.Inconclusive();
			}
			var user = GetRandomUser(out var password, out var context);
			var schema = SchemaGenerator.GetRandomSchema<T>(context);
			var factory = ResourceUtility.GetFactory<T>();
			var rsc = factory.CreateTypedObject(context, schema);
			var filter = new DistanceFilter() { latlong = $"{rsc.Location.Latitude} {rsc.Location.Longitude}", distance = 20 };
			var manager = ResourceUtility.GetManager(typeof(T));
			var search = manager.Get(x => filter.Filter(x));
			Assert.IsTrue(search.Single().GUID == rsc.GUID);
		}
	}
}
