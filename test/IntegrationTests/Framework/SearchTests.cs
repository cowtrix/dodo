using Dodo;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.LocationResources;
using Dodo;
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
			var eventSite = CreateObject<Event>(seed: false);
			using (var rscLock = new ResourceLock(eventSite))
			{
				eventSite.StartDate = startDate + TimeSpan.FromDays(1);
				eventSite.EndDate = eventSite.StartDate + TimeSpan.FromHours(4);
				ResourceUtility.GetManager<Event>().Update(eventSite, rscLock);
			}
			eventSite = ResourceUtility.GetManager<Event>().GetSingle(r => r.Guid == eventSite.Guid) as Event;
			var marchSite = CreateObject<Event>(seed: false);
			using (var rscLock = new ResourceLock(marchSite))
			{
				marchSite.StartDate = endDate - TimeSpan.FromDays(1);
				marchSite.EndDate = marchSite.StartDate + TimeSpan.FromHours(4);
				ResourceUtility.GetManager<Event>().Update(marchSite, rscLock);
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
				CreateObject<Site>(seed: false),
				CreateObject<Event>(seed: false),
				CreateObject<LocalGroup>(seed: false)
			};
			var request = await RequestJSON<JArray>($"{Dodo.DodoApp.API_ROOT}{SearchAPIController.RootURL}", EHTTPRequestType.GET, null,
				new[]
				{
					("startDate", startDate.ToString()),
					("endDate", endDate.ToString())
				});
			var guids = request.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase())).Select(s => Guid.Parse(s));
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

		[TestMethod]
		public async Task CanSearchByParent()
		{
			GetRandomUser(out _, out var context);
			var rebellion1 = CreateObject<Rebellion>(context);
			var positives = new List<IOwnedResource>()
			{
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 1", "", rebellion1.Guid)),
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 2", "", rebellion1.Guid)),
				CreateObject<Event>(context, new EventSchema("Test Event Site", rebellion1.Guid, new GeoLocation(rebellion1.Location).Offset(-.05, .1), "", rebellion1.StartDate, rebellion1.StartDate), false),
				CreateObject<Event>(context, new EventSchema("Test March Site", rebellion1.Guid, new GeoLocation(rebellion1.Location).Offset(-.05, .1), "", rebellion1.StartDate, rebellion1.StartDate), false),
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
			var request = await RequestJSON<JArray>($"{Dodo.DodoApp.API_ROOT}{SearchAPIController.RootURL}", EHTTPRequestType.GET, null,
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
			string magic = "asjdnajdwbakjfbskdfb.......sdfkjsfse";
			GetRandomUser(out _, out var context);
			var rebellion1 = CreateObject<Rebellion>(context);
			var positives = new List<IRESTResource>()
			{
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema($"{magic} Working Group 1", "", rebellion1.Guid)),
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema($"{magic} Working Group 2", "", rebellion1.Guid)),
				CreateObject<Event>(context, new EventSchema($"Test Event {magic}", rebellion1.Guid, SchemaGenerator.RandomLocation, "", rebellion1.StartDate, rebellion1.StartDate), false),
				CreateObject<Event>(context, new EventSchema($"Test {magic} Site", rebellion1.Guid, SchemaGenerator.RandomLocation, "", rebellion1.StartDate, rebellion1.StartDate), false),
			};
			var negatives = new List<IRESTResource>()
			{
				CreateObject<Rebellion>(),
				CreateObject<Site>(),
				CreateObject<Event>(),
				CreateObject<LocalGroup>(),
				CreateObject<WorkingGroup>(),
			};
			var request = await RequestJSON<JArray>($"{Dodo.DodoApp.API_ROOT}{SearchAPIController.RootURL}", EHTTPRequestType.GET, null,
				new[]
				{
					("search", magic ),
				});
			var guids = request.Values<JObject>().Select(o => o.Value<string>(nameof(IRESTResource.Guid).ToCamelCase())).Select(s => Guid.Parse(s));
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

		[TestMethod]
		public async Task CanSearchByType()
		{
			GetRandomUser(out _, out var context);
			var rebellion1 = CreateObject<Rebellion>(context);
			var positives = new List<IOwnedResource>()
			{
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 1", "", rebellion1.Guid)),
				CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test Working Group 2", "", rebellion1.Guid)),
				CreateObject<Event>(context, new EventSchema("Test Event Site", rebellion1.Guid, SchemaGenerator.RandomLocation, "", rebellion1.StartDate, rebellion1.StartDate), false),
				CreateObject<Event>(context, new EventSchema("Test March Site", rebellion1.Guid, SchemaGenerator.RandomLocation, "", rebellion1.StartDate, rebellion1.StartDate), false),
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
			var request = await RequestJSON<JArray>($"{Dodo.DodoApp.API_ROOT}{SearchAPIController.RootURL}", EHTTPRequestType.GET, null,
				new[]
				{
					("types", $"{nameof(WorkingGroup).ToLowerInvariant()},{nameof(Event).ToLowerInvariant()}"),
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
			var rebellion1 = CreateObject<Rebellion>(context);
			var positives = new ILocationalResource[] {
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 1", "", new GeoLocation(rebellion1.Location).Offset(-.1, -.1))),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 2", "", new GeoLocation(rebellion1.Location).Offset(.1, -.1))),
				CreateObject<Event>(context, new EventSchema("Test Event Site", rebellion1.Guid, new GeoLocation(rebellion1.Location).Offset(-.05, .1), "", rebellion1.StartDate, rebellion1.StartDate), false),
				CreateObject<Event>(context, new EventSchema("Test March Site", rebellion1.Guid, new GeoLocation(rebellion1.Location).Offset(-.05, .1), "", rebellion1.StartDate, rebellion1.StartDate), false),
				};
			var negatives = new ILocationalResource[] {
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 1", "", new GeoLocation(rebellion1.Location).Offset(-45, -45))),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 2", "", new GeoLocation(rebellion1.Location).Offset(-45, -45))),
				CreateObject<Event>(context, new EventSchema("Test Event Site", rebellion1.Guid, new GeoLocation(rebellion1.Location).Offset(-45, -45), "", rebellion1.StartDate, rebellion1.StartDate), false),
				CreateObject<Event>(context, new EventSchema("Test March Site", rebellion1.Guid, new GeoLocation(rebellion1.Location).Offset(-45, -45), "", rebellion1.StartDate, rebellion1.StartDate), false),
				};
			var request = await RequestJSON<JArray>($"{Dodo.DodoApp.API_ROOT}{SearchAPIController.RootURL}", EHTTPRequestType.GET, null,
				new[]
				{
					("latlong", $"{rebellion1.Location.Latitude}+{rebellion1.Location.Longitude}"),
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
