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

namespace RESTTests.Search
{
	[TestClass]
	public class GenericSearchTests : IntegrationTestBase 
	{
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
			var posGuids = positives.Select(rsc => rsc.Guid).ToList();
			var negatives = new ILocationalResource[] {
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 1", "", new GeoLocation(rebellion.Location).Offset(-45, -45))),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 2", "", new GeoLocation(rebellion.Location).Offset(-45, -45))),
				CreateObject<MarchSite>(context, new SiteSchema("My March", typeof(MarchSite).FullName, rebellion.Guid, new GeoLocation(rebellion.Location).Offset(-45, 45), "")),
				CreateObject<PermanentSite>(context, new SiteSchema("My Occupation", typeof(PermanentSite).FullName, rebellion.Guid, new GeoLocation(rebellion.Location).Offset(45, 45), "")),
				};
			var negGuids = negatives.Select(rsc => rsc.Guid).ToList();
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
