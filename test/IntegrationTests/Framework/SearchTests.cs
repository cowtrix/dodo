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

namespace RESTTests
{
	[TestClass]
	public class SearchTests : IntegrationTestBase 
	{
		[TestMethod]
		public async Task CanSearchByLocation()
		{
			var admin = GetRandomUser(out var password, out var context);
			var rebellion = CreateObject<Rebellion>(context);
			var positives = new ILocationalResource[] {
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 1", "", new GeoLocation(rebellion.Location).Offset(-.1, -.1))),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 2", "", new GeoLocation(rebellion.Location).Offset(.1, -.1))),
				CreateObject<MarchSite>(context, new SiteSchema("My March", typeof(MarchSite).FullName, rebellion.GUID, new GeoLocation(rebellion.Location).Offset(-.05, .1), "")),
				CreateObject<OccupationSite>(context, new SiteSchema("My Occupation", typeof(OccupationSite).FullName, rebellion.GUID, new GeoLocation(rebellion.Location).Offset(.05, -.1), "")),
				};
			var negatives = new ILocationalResource[] {
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 1", "", new GeoLocation(rebellion.Location).Offset(-45, -45))),
				CreateObject<LocalGroup>(context, new LocalGroupSchema("My Local Group 2", "", new GeoLocation(rebellion.Location).Offset(-45, -45))),
				CreateObject<MarchSite>(context, new SiteSchema("My March", typeof(MarchSite).FullName, rebellion.GUID, new GeoLocation(rebellion.Location).Offset(-45, 45), "")),
				CreateObject<OccupationSite>(context, new SiteSchema("My Occupation", typeof(OccupationSite).FullName, rebellion.GUID, new GeoLocation(rebellion.Location).Offset(45, 45), "")),
				};

			var request = await RequestJSON<JArray>($"{SearchController.RootUrl}", EHTTPRequestType.GET, null,
				new[]
				{
					("latlong", $"{rebellion.Location.Latitude}+{rebellion.Location.Longitude}"),
					("distance", "60"),
				});
			var guids = request.Values<JObject>().Select(o => o.Value<string>("GUID"));
			foreach(var guid in guids)
			{
				var match = positives.SingleOrDefault(rsc => rsc.GUID == Guid.Parse(guid));
				Assert.IsNotNull(match, $"Unable to find resource with GUID {guid}");
			}
		}
	}
}
