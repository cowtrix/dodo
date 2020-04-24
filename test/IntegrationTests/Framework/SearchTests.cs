using Dodo;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Sites;
using DodoResources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Resources;
using System;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using Resources.Location;
using Dodo.WorkingGroups;
using System.Collections.Generic;
using Dodo.SharedTest;

namespace RESTTests.Search
{
	[TestClass]
	public class GenericSearchTests : IntegrationTestBase 
	{
		[TestMethod]
		public async Task CanSearchByDate()
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
			var request = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{SearchController.RootURL}", EHTTPRequestType.GET, null,
				new[]
				{
					("startDate", startDate.ToString()),
					("endDate", endDate.ToString())
				});
			var guids = request.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			foreach (var guid in guids)
			{
				if (rebellion1.Guid == Guid.Parse(guid))
				{
					continue;
				}
				var match = positives.SingleOrDefault(rsc => rsc.Guid == Guid.Parse(guid));
				Assert.IsNotNull(match, $"Unable to find resource with GUID {guid}");
			}
		}

		[TestMethod]
		public async Task CanSearchByParent()
		{
			GetRandomUser(out _, out var context);
			var rebellion1 = CreateObject<Rebellion>(context);
			var positives = new List<IOwnedResource>()
			{
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 1", "", rebellion1.Guid)),
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 2", "", rebellion1.Guid)),
				CreateObject<Site>(context, new SiteSchema("Test Event Site", typeof(EventSite).FullName, rebellion1.Guid, SchemaGenerator.RandomLocation, "")),
				CreateObject<Site>(context, new SiteSchema("Test March Site", typeof(MarchSite).FullName, rebellion1.Guid, SchemaGenerator.RandomLocation, "")),
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
			var request = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{SearchController.RootURL}", EHTTPRequestType.GET, null,
				new[]
				{
					("parent", rebellion1.Guid.ToString()),
				});
			var guids = request.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			foreach (var guid in guids)
			{
				if (rebellion1.Guid == Guid.Parse(guid))
				{
					continue;
				}
				var match = positives.SingleOrDefault(rsc => rsc.Guid == Guid.Parse(guid));
				Assert.IsNotNull(match, $"Unable to find resource with GUID {guid}");
			}
		}

		[TestMethod]
		public async Task CanSearchByString()
		{
			GetRandomUser(out _, out var context);
			var rebellion1 = CreateObject<Rebellion>(context);
			var positives = new List<IOwnedResource>()
			{
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 1", "", rebellion1.Guid)),
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 2", "", rebellion1.Guid)),
				CreateObject<Site>(context, new SiteSchema("Test Event Site", typeof(EventSite).FullName, rebellion1.Guid, SchemaGenerator.RandomLocation, "")),
				CreateObject<Site>(context, new SiteSchema("Test March Site", typeof(MarchSite).FullName, rebellion1.Guid, SchemaGenerator.RandomLocation, "")),
			};
			var negativeResults = new List<IRESTResource>()
			{
				rebellion1,
				CreateObject<Rebellion>(),
				CreateObject<PermanentSite>(),
				CreateObject<EventSite>(),
				CreateObject<LocalGroup>(),
				CreateObject<WorkingGroup>(),
			};
			var request = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{SearchController.RootURL}", EHTTPRequestType.GET, null,
				new[]
				{
					("search", "test"),
				});
			var guids = request.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			foreach (var guid in guids)
			{
				if (rebellion1.Guid == Guid.Parse(guid))
				{
					continue;
				}
				var match = positives.SingleOrDefault(rsc => rsc.Guid == Guid.Parse(guid));
				Assert.IsNotNull(match, $"Unable to find resource with GUID {guid}");
			}
		}

		[TestMethod]
		public async Task CanSearchByLocation()
		{
			var admin = GetRandomUser(out var password, out var context);
			var rebellion = CreateObject<Rebellion>(context);
			var positives = new ILocationalResource[] {
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 1", "", new GeoLocation(rebellion.Location).Offset(-.1, -.1))),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 2", "", new GeoLocation(rebellion.Location).Offset(.1, -.1))),
				CreateObject<MarchSite>(context, new SiteSchema("My March", typeof(MarchSite).FullName, rebellion.Guid, new GeoLocation(rebellion.Location).Offset(-.05, .1), "")),
				CreateObject<PermanentSite>(context, new SiteSchema("My Occupation", typeof(PermanentSite).FullName, rebellion.Guid, new GeoLocation(rebellion.Location).Offset(.05, -.1), "")),
				};
			var negatives = new ILocationalResource[] {
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 1", "", new GeoLocation(rebellion.Location).Offset(-45, -45))),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 2", "", new GeoLocation(rebellion.Location).Offset(-45, -45))),
				CreateObject<MarchSite>(context, new SiteSchema("My March", typeof(MarchSite).FullName, rebellion.Guid, new GeoLocation(rebellion.Location).Offset(-45, 45), "")),
				CreateObject<PermanentSite>(context, new SiteSchema("My Occupation", typeof(PermanentSite).FullName, rebellion.Guid, new GeoLocation(rebellion.Location).Offset(45, 45), "")),
				};
			var request = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{SearchController.RootURL}", EHTTPRequestType.GET, null,
				new[]
				{
					("latlong", $"{rebellion.Location.Latitude}+{rebellion.Location.Longitude}"),
					("distance", "60"),
				});
			var guids = request.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase()));
			foreach(var guid in guids)
			{
				if(rebellion.Guid == Guid.Parse(guid))
				{
					continue;
				}
				var match = positives.SingleOrDefault(rsc => rsc.Guid == Guid.Parse(guid));
				Assert.IsNotNull(match, $"Unable to find resource with GUID {guid}");
			}
		}
	}
}
