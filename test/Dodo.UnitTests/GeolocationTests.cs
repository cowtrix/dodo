using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Resources;
using Resources.Location;
using SharedTest;
using System.Threading.Tasks;

namespace Dodo.UnitTests
{
	[TestClass]
	public class GeolocationTests : TestBase
	{
		[TestInitialize]
		public void Setup()
		{
			LocationManager.ClearCache();
		}

		class MyClass
		{
			[View(EPermissionLevel.PUBLIC)]
			public GeoLocation Location;
		}

		[TestMethod]
		public async Task CanReverseGeocodeFromGeolocation()
		{
			if(!LocationManager.Enabled)
			{
				Assert.Inconclusive();
			}
			var location = new GeoLocation(52.370216, 4.895168);     // Amsterdam
			var locationData = await LocationManager.GetLocationDataAsync(location);
			Assert.AreEqual("Netherlands", locationData.Country);
			Assert.AreEqual("Amsterdam", locationData.Place);
			Assert.AreEqual("1012 EG", locationData.Postcode);

			var locDataJson = JsonConvert.SerializeObject(JsonViewUtility.GenerateJsonView(new MyClass { Location = location }, EPermissionLevel.PUBLIC, null, default), Common.Extensions.JsonExtensions.NetworkSettings);
		}
	}

}
