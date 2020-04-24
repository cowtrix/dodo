using Dodo;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Resources;
using Dodo.SharedTest;
using Dodo.Sites;
using GenerateSampleData;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using Resources.Location;
using SharedTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
				CreateObject<PermanentSite>(context, new SiteSchema("Test Site 1", typeof(PermanentSite).FullName, rebellion.Guid, new GeoLocation(TrueLocation.Latitude + .1, TrueLocation.Longitude), "")),
				CreateObject<EventSite>(context, new SiteSchema("Test Site 2", typeof(EventSite).FullName, rebellion.Guid, new GeoLocation(TrueLocation.Latitude - .1, TrueLocation.Longitude +.1), "")),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("Test LG 1", "", TrueLocation))
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
			var startDate = SchemaGenerator.RandomDate;
			var endDate = startDate + TimeSpan.FromDays(7);
			GetRandomUser(out _, out var context);
			var rebellion1 = CreateObject<Rebellion>(context, new RebellionSchema("Test", "",
				SchemaGenerator.RandomLocation, startDate + TimeSpan.FromDays(1), endDate + TimeSpan.FromDays(4)));
			var evt1 = CreateObject<EventSite>();
			using (var rscLock = new ResourceLock(evt1))
			{
				evt1.StartDate = startDate + TimeSpan.FromDays(1);
				evt1.EndDate = startDate + TimeSpan.FromHours(4);
				ResourceUtility.GetManager<Site>().Update(evt1, rscLock);
			}
			evt1 = ResourceUtility.GetManager<Site>().GetSingle(r => r.Guid == evt1.Guid) as EventSite;
			var evt2 = CreateObject<MarchSite>();
			using (var rscLock = new ResourceLock(evt2))
			{
				evt2.StartDate = endDate - TimeSpan.FromDays(1);
				evt2.EndDate = evt2.StartDate + TimeSpan.FromHours(4);
				ResourceUtility.GetManager<Site>().Update(evt2, rscLock);
			}
			var goodResults = new List<ITimeBoundResource>()
			{
				rebellion1,
				evt1,
				evt2,
			};
			/*var negativeResults = new List<IRESTResource>()
			{
				CreateObject<Rebellion>(),
				CreateObject<PermanentSite>(),
				CreateObject<EventSite>(),
				CreateObject<LocalGroup>()
			};*/
			var result = DodoResourceUtility.Search(0, 100, new DateFilter() { StartDate = startDate.ToString(), EndDate = endDate.ToString() });
			Assert.AreEqual(goodResults.Count, result.Count());
			Assert.IsTrue(result.All(r => goodResults.Contains(r)));
			//Assert.IsFalse(result.All(r => negativeResults.Contains(r)));
		}
	}
}
