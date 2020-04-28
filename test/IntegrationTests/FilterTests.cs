using Dodo;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Resources;
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
	public class FilterTests : TestBase
	{
		[TestMethod]
		public void FilterRebellions()
		{
			FilterByDate<Rebellion>();
			FilterByLocation<Rebellion>();
		}

		[TestMethod]
		public void FilterLocalGroups()
		{
			FilterByLocation<LocalGroup>();
		}

		public void FilterByDate<T>() where T : GroupResource, ITimeBoundResource
		{
			if(typeof(T).IsAssignableFrom(typeof(ITimeBoundResource)))
			{
				Assert.Inconclusive();
			}
			var user = GetRandomUser(out var password, out var context);
			var schema = SchemaGenerator.GetRandomSchema<T>(context);
			var factory = ResourceUtility.GetFactory<T>();
			var rsc = factory.CreateTypedObject(context, schema);
			var filter = new DateFilter() { StartDate = rsc.StartDate.ToShortDateString() };
			var manager = ResourceUtility.GetManager(typeof(T));
			var search = manager.Get(x => filter.Filter(x));
			Assert.IsTrue(search.Single().Guid == rsc.Guid);
		}

		public void FilterByLocation<T>() where T : GroupResource, ILocationalResource
		{
			if (typeof(T).IsAssignableFrom(typeof(ILocationalResource)))
			{
				Assert.Inconclusive();
			}
			var user = GetRandomUser(out var password, out var context);
			var schema = SchemaGenerator.GetRandomSchema<T>(context);
			var factory = ResourceUtility.GetFactory<T>();
			var rsc = factory.CreateTypedObject(context, schema);
			var filter = new DistanceFilter() { LatLong = $"{rsc.Location.Latitude} {rsc.Location.Longitude}", Distance = 20 };
			var manager = ResourceUtility.GetManager(typeof(T));
			var search = manager.Get(x => filter.Filter(x));
			Assert.IsTrue(search.Single().Guid == rsc.Guid);
		}
	}
}
