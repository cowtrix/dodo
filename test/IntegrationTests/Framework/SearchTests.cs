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
using Dodo.Roles;

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
				SchemaGenerator.RandomLocation, startDate + TimeSpan.FromDays(1), endDate + TimeSpan.FromDays(4)), false);
			var eventSite = CreateObject<EventSite>(seed: false);
			using (var rscLock = new ResourceLock(eventSite))
			{
				eventSite.StartDate = startDate + TimeSpan.FromDays(1);
				eventSite.EndDate = eventSite.StartDate + TimeSpan.FromHours(4);
				ResourceUtility.GetManager<Site>().Update(eventSite, rscLock);
			}
			eventSite = ResourceUtility.GetManager<Site>().GetSingle(r => r.Guid == eventSite.Guid) as EventSite;
			var marchSite = CreateObject<MarchSite>(seed: false);
			using (var rscLock = new ResourceLock(marchSite))
			{
				marchSite.StartDate = endDate - TimeSpan.FromDays(1);
				marchSite.EndDate = marchSite.StartDate + TimeSpan.FromHours(4);
				ResourceUtility.GetManager<Site>().Update(marchSite, rscLock);
			}
			var positives = new List<ITimeBoundResource>()
			{
				rebellion1,
				eventSite,
				marchSite,
			};
			var negatives = new List<IRESTResource>()
			{
				CreateObject<Rebellion>(seed: false),
				CreateObject<PermanentSite>(seed: false),
				CreateObject<EventSite>(seed: false),
				CreateObject<LocalGroup>(seed: false)
			};
			var request = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{SearchController.RootURL}", EHTTPRequestType.GET, null,
				new[]
				{
					("startDate", startDate.ToString()),
					("endDate", endDate.ToString())
				});
			var guids = request.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase())).Select(s => Guid.Parse(s));
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
			var guids = request.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase())).Select(s => Guid.Parse(s));
			foreach(var pos in positives)
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
					("search", "test"),
				});
			var guids = request.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase())).Select(s => Guid.Parse(s));
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
		public async Task CanSearchByType()
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
				CreateObject<LocalGroup>(),
				CreateObject<LocalGroup>(),
				CreateObject<Role>(),
				CreateObject<Role>(),
			};
			var request = await RequestJSON<JArray>($"{DodoServer.DodoServer.API_ROOT}{SearchController.RootURL}", EHTTPRequestType.GET, null,
				new[]
				{
					("types", "workinggroup,eventsite,marchsite"),
				});
			var guids = request.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase())).Select(s => Guid.Parse(s));
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
			var guids = request.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase())).Select(s => Guid.Parse(s));
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
