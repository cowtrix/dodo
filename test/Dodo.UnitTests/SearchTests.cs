using Dodo;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.DodoResources;
using Dodo.SharedTest;
using Dodo.LocationResources;
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
			var baseDate = SchemaGenerator.RandomDate;
			GetRandomUser(out _, out var context);
			var rebellion = CreateObject<Rebellion>(context, new RebellionSchema("Test", "", TrueLocation, SchemaGenerator.RandomDate, SchemaGenerator.RandomDate));
			var positives = new List<ILocationalResource>()
			{
				rebellion,
				CreateObject<Site>(context, new SiteSchema("Test Site 1", rebellion.Guid.ToString(), new GeoLocation(TrueLocation.Latitude + .1, TrueLocation.Longitude), "", SchemaGenerator.RandomFacilities, SchemaGenerator.RandomVideoURL), false),
				CreateObject<Event>(context, new EventSchema("Test Site 2", rebellion.Guid.ToString(), new GeoLocation(TrueLocation.Latitude - .1, TrueLocation.Longitude +.1), "", SchemaGenerator.RandomFacilities, SchemaGenerator.RandomVideoURL, baseDate, baseDate), false),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("Test LG 1", "", TrueLocation), false)
			};
			
			var negatives = new List<ILocationalResource>()
			{
				CreateObject<Rebellion>(),
				CreateObject<Site>(context, new SiteSchema("Test Site 1", rebellion.Guid.ToString(), new GeoLocation(TrueLocation.Longitude + .1, TrueLocation.Latitude), "", SchemaGenerator.RandomFacilities, SchemaGenerator.RandomVideoURL)),
				CreateObject<Event>(context, new EventSchema("Test Site 2", rebellion.Guid.ToString(), new GeoLocation(TrueLocation.Longitude - .1, TrueLocation.Latitude +.1), "", SchemaGenerator.RandomFacilities, SchemaGenerator.RandomVideoURL, baseDate, baseDate)),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("Test LG 1", "", new GeoLocation(TrueLocation.Longitude - .1, TrueLocation.Latitude +.1)))
			};
			var guids = DodoResourceUtility.Search(0, 100, false, new DistanceFilter() { LatLong = $"{TrueLocation.Latitude} {TrueLocation.Longitude}", Distance = 100 })
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
		public void FilterByDate()
		{
			var startDate = SchemaGenerator.RandomDate.ToUniversalTime();
			var endDate = startDate + TimeSpan.FromDays(7);
			GetRandomUser(out _, out var context);
			var rebellion1 = CreateObject<Rebellion>(context, new RebellionSchema("Test", "",
				SchemaGenerator.RandomLocation, startDate + TimeSpan.FromDays(1), endDate + TimeSpan.FromDays(4)), seed:false);
			var evt1 = CreateObject<Event>(seed: false);
			using (var rscLock = new ResourceLock(evt1))
			{
				evt1.Name = "test";
				evt1.StartDate = startDate + TimeSpan.FromDays(1);
				evt1.EndDate = evt1.StartDate + TimeSpan.FromHours(4);
				ResourceUtility.GetManager<Event>().Update(evt1, rscLock);
			}
			var evt2 = CreateObject<Event>(seed: false);
			using (var rscLock = new ResourceLock(evt2))
			{
				evt2.StartDate = endDate - TimeSpan.FromDays(1);
				evt2.EndDate = evt2.StartDate + TimeSpan.FromHours(4);
				ResourceUtility.GetManager<Event>().Update(evt2, rscLock);
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
				CreateObject<Site>(),
				CreateObject<Event>(),
				CreateObject<LocalGroup>()
			};
			var guids = DodoResourceUtility.Search(0, 100, false, new DateFilter() { StartDate = startDate.ToString(), EndDate = endDate.ToString() })
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
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 1", "", rebellion1.Guid.ToString()), false),
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 2", "", rebellion1.Guid.ToString()), false),
				CreateObject<Event>(context, new EventSchema("Test Event Site", rebellion1.Guid.ToString(), SchemaGenerator.RandomLocation, "", SchemaGenerator.RandomFacilities, SchemaGenerator.RandomVideoURL, rebellion1.StartDate, rebellion1.EndDate), false),
				CreateObject<Event>(context, new EventSchema("Test March Site", rebellion1.Guid.ToString(), SchemaGenerator.RandomLocation, "", SchemaGenerator.RandomFacilities, SchemaGenerator.RandomVideoURL, rebellion1.StartDate, rebellion1.EndDate), false),
			};
			var negatives = new List<IRESTResource>()
			{
				rebellion1,
				CreateObject<Rebellion>(),
				CreateObject<Site>(),
				CreateObject<Event>(),
				CreateObject<LocalGroup>(),
				CreateObject<WorkingGroup>(),
			};
			var guids = DodoResourceUtility.Search(0, 100, false, new ParentFilter() { Parent = rebellion1.Guid.ToString() })
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
			var positives = new List<IRESTResource>()
			{
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working asfafsd 1", "", rebellion1.Guid.ToString()), false),
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("asfafsd Working Group 2", "", rebellion1.Guid.ToString()), false),
				CreateObject<Event>(context, new EventSchema("Test asfafsd Site", rebellion1.Guid.ToString(), SchemaGenerator.RandomLocation, "", SchemaGenerator.RandomFacilities, SchemaGenerator.RandomVideoURL, rebellion1.StartDate, rebellion1.StartDate), false),
				CreateObject<Event>(context, new EventSchema("Test March asfafsd", rebellion1.Guid.ToString(), SchemaGenerator.RandomLocation, "", SchemaGenerator.RandomFacilities, SchemaGenerator.RandomVideoURL, rebellion1.StartDate, rebellion1.StartDate), false),
			};
			var negatives = new List<IRESTResource>()
			{
				CreateObject<Rebellion>(),
				CreateObject<Site>(),
				CreateObject<Event>(),
				CreateObject<LocalGroup>(),
				CreateObject<WorkingGroup>(),
			};
			var guids = DodoResourceUtility.Search(0, 100, false, new StringFilter() { Search = "asfafsd" })
				.Select(r => r.Guid);
			foreach (var pos in positives)
			{
				Assert.IsTrue(guids.Contains(pos.Guid),
					$"Results did not contain expected resource: {pos} {pos.GetType()}");
			}
			foreach (var neg in negatives)
			{
				Assert.IsFalse(guids.Contains(neg.Guid),
					$"Results contained unexpected resource: {neg} {neg.GetType()}");
			}
		}
	}
}
