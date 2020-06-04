using Dodo;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Resources;
using Dodo.SharedTest;
using Dodo.Sites;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using Resources.Location;
using SharedTest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchTests
{
	[TestClass]
	public class SearchTests : TestBase
	{
		[TestMethod]
		public void FilterByLocation()
		{
			GeoLocation TrueLocation = new GeoLocation(52.428, 4.9500000000000455);

			GetRandomUser(out _, out var context);
			var rebellion = CreateObject<Rebellion>(context, new RebellionSchema("Test", "", TrueLocation, SchemaGenerator.RandomDate, SchemaGenerator.RandomDate));
			var goodResults = new List<ILocationalResource>()
			{
				rebellion,
				CreateObject<PermanentSite>(context, new SiteSchema("Test Site 1", typeof(PermanentSite).FullName, rebellion.Guid, new GeoLocation(TrueLocation.Latitude + .1, TrueLocation.Longitude), ""), false),
				CreateObject<EventSite>(context, new SiteSchema("Test Site 2", typeof(EventSite).FullName, rebellion.Guid, new GeoLocation(TrueLocation.Latitude - .1, TrueLocation.Longitude +.1), ""), false),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("Test LG 1", "", TrueLocation), false)
			};
			var negativeResults = new List<ILocationalResource>()
			{
				CreateObject<Rebellion>(),
				CreateObject<PermanentSite>(context, new SiteSchema("Test Site 1", typeof(PermanentSite).FullName, rebellion.Guid, new GeoLocation(TrueLocation.Longitude + .1, TrueLocation.Latitude), "")),
				CreateObject<EventSite>(context, new SiteSchema("Test Site 2", typeof(EventSite).FullName, rebellion.Guid, new GeoLocation(TrueLocation.Longitude - .1, TrueLocation.Latitude +.1), "")),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("Test LG 1", "", new GeoLocation(TrueLocation.Longitude - .1, TrueLocation.Latitude +.1)))
			};
			var result = DodoResourceUtility.Search(0, 100, new DistanceFilter() { LatLong = $"{TrueLocation.Latitude} {TrueLocation.Longitude}", Distance = 100 });
			Assert.AreEqual(goodResults.Count, result.Count());
			Assert.IsTrue(result.All(r => goodResults.Contains(r)));
			Assert.IsFalse(result.All(r => negativeResults.Contains(r)));
		}

		[TestMethod]
		public void FilterByDate()
		{
			var startDate = SchemaGenerator.RandomDate.ToUniversalTime();
			var endDate = startDate + TimeSpan.FromDays(7);
			GetRandomUser(out _, out var context);
			var rebellion1 = CreateObject<Rebellion>(context, new RebellionSchema("Test", "",
				SchemaGenerator.RandomLocation, startDate + TimeSpan.FromDays(1), endDate + TimeSpan.FromDays(4)), seed:false);
			var evt1 = CreateObject<EventSite>(seed: false);
			using (var rscLock = new ResourceLock(evt1))
			{
				evt1.Name = "test";
				evt1.StartDate = startDate + TimeSpan.FromDays(1);
				evt1.EndDate = evt1.StartDate + TimeSpan.FromHours(4);
				ResourceUtility.GetManager<Site>().Update(evt1, rscLock);
			}
			var evt2 = CreateObject<MarchSite>(seed: false);
			using (var rscLock = new ResourceLock(evt2))
			{
				evt2.StartDate = endDate - TimeSpan.FromDays(1);
				evt2.EndDate = evt2.StartDate + TimeSpan.FromHours(4);
				ResourceUtility.GetManager<Site>().Update(evt2, rscLock);
			}
			var positives = new List<ITimeBoundResource>()
			{
				rebellion1,
				evt1,
				evt2,
			};
			var negatives = new List<IRESTResource>()
			{
				CreateObject<Rebellion>(),
				CreateObject<PermanentSite>(),
				CreateObject<EventSite>(),
				CreateObject<LocalGroup>()
			};
			var guids = DodoResourceUtility.Search(0, 100, new DateFilter() { StartDate = startDate.ToString(), EndDate = endDate.ToString() })
				.Select(r => r.Guid).ToList();
			var allResources = ResourceUtility.ResourceManagers.SelectMany(rm => rm.Value.Get(r => true)).ToList();
			foreach (var pos in positives)
			{
				Assert.IsTrue(guids.Contains(pos.Guid),
					$"Results did not contain expected resource: {pos.GetType()}");
			}
			foreach (var neg in negatives)
			{
				Assert.IsFalse(guids.Contains(neg.Guid),
					$"Results contained unexpected resource: {neg.GetType()}");
			}
		}

		[TestMethod]
		public void FilterByParent()
		{
			GetRandomUser(out _, out var context);
			var rebellion1 = CreateObject<Rebellion>(context, seed: false);
			var positives = new List<IOwnedResource>()
			{
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 1", "", rebellion1.Guid), false),
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 2", "", rebellion1.Guid), false),
				CreateObject<Site>(context, new SiteSchema("Test Event Site", typeof(EventSite).FullName, rebellion1.Guid, SchemaGenerator.RandomLocation, ""), false),
				CreateObject<Site>(context, new SiteSchema("Test March Site", typeof(MarchSite).FullName, rebellion1.Guid, SchemaGenerator.RandomLocation, ""), false),
			};
			var negatives = new List<IRESTResource>()
			{
				rebellion1,
				CreateObject<Rebellion>(),
				CreateObject<PermanentSite>(),
				CreateObject<EventSite>(),
				CreateObject<LocalGroup>(),
				CreateObject<WorkingGroup>(),
			};
			var guids = DodoResourceUtility.Search(0, 100, new ParentFilter() { Parent = rebellion1.Guid })
				.Select(r => r.Guid);
			foreach (var pos in positives)
			{
				Assert.IsTrue(guids.Contains(pos.Guid),
					$"Results did not contain expected resource: {pos.GetType()}");
			}
			foreach (var neg in negatives)
			{
				Assert.IsFalse(guids.Contains(neg.Guid),
					$"Results contained unexpected resource: {neg.GetType()}");
			}
		}

		[TestMethod]
		public void FilterByString()
		{
			GetRandomUser(out _, out var context);
			var rebellion1 = CreateObject<Rebellion>(context);
			var positives = new List<IOwnedResource>()
			{
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 1", "", rebellion1.Guid), false),
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 2", "", rebellion1.Guid), false),
				CreateObject<Site>(context, new SiteSchema("Test Event Site", typeof(EventSite).FullName, rebellion1.Guid, SchemaGenerator.RandomLocation, ""), false),
				CreateObject<Site>(context, new SiteSchema("Test March Site", typeof(MarchSite).FullName, rebellion1.Guid, SchemaGenerator.RandomLocation, ""), false),
			};
			var negatives = new List<IRESTResource>()
			{
				rebellion1,
				CreateObject<Rebellion>(),
				CreateObject<PermanentSite>(),
				CreateObject<EventSite>(),
				CreateObject<LocalGroup>(),
				CreateObject<WorkingGroup>(),
			};
			var guids = DodoResourceUtility.Search(0, 100, new StringFilter() { Search = "Test" })
				.Select(r => r.Guid);
			foreach (var pos in positives)
			{
				Assert.IsTrue(guids.Contains(pos.Guid),
					$"Results did not contain expected resource: {pos.GetType()}");
			}
			foreach (var neg in negatives)
			{
				Assert.IsFalse(guids.Contains(neg.Guid),
					$"Results contained unexpected resource: {neg.GetType()}");
			}
		}
	}
}
