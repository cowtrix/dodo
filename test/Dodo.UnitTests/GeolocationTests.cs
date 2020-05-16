using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Resources;
using Resources.Location;
using SharedTest;
using System.Threading.Tasks;

namespace UnitTests
{
	[TestClass]
	public class GeolocationTests : TestBase
	{
		[TestInitialize]
		public void Setup()
		{
			LocationManager.ClearCache();
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
		}

		[TestMethod]
		public async Task CanGeocodeFromString()
		{
			if (!LocationManager.Enabled)
			{
				Assert.Inconclusive();
			}
			var searchString = "Amsterdam";
			var expectedLocation = new GeoLocation(52.370216, 4.895168);
			var location = await LocationManager.GetLocation(searchString);
			var distance = expectedLocation.ToCoordinate().GetDistanceTo(location.ToCoordinate());
			Assert.IsTrue(distance < 1000, $"Distance was too high: {distance}m");
		}
	}

}
