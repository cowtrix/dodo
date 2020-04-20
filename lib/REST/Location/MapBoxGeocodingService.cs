using Common;
using Common.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Resources.Location
{
	public class MapBoxGeocodingService : IGeocodingService
	{
		static Uri m_baseUrl = new Uri("https://api.mapbox.com");
		static string ApiKey => ConfigManager.GetValue($"{nameof(MapBoxGeocodingService)}_{nameof(ApiKey)}", "");

		public bool Enabled => !string.IsNullOrEmpty(ApiKey);

		static HttpClient m_httpClient;

		static MapBoxGeocodingService()
		{
			m_httpClient = new HttpClient() { BaseAddress = m_baseUrl };
		}

		public async Task<LocationData> GetLocationData(GeoLocation location)
		{
			string GetFromFeatures(JArray arr, string key)
			{
				return arr.FirstOrDefault(j => j.Value<JArray>("place_type").Values<string>().Contains(key))?.Value<string>("text");
			}
			if(string.IsNullOrEmpty(ApiKey))
			{
				Logger.Warning($"No MapBox API Key is set.");
				return null;
			}
			var response = await m_httpClient.GetAsync(
				"/geocoding/v5/mapbox.places/" +	// TODO: change to permanent?
				$"{location.Longitude},{location.Latitude}.json" +
				$"?access_token={ApiKey}");
			var body = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				Logger.Error($"{nameof(MapBoxGeocodingService)}: {response.StatusCode}\n{body}");
				return null;
			}
			var json = JsonConvert.DeserializeObject<JObject>(body);
			var features = json.Value<JArray>("features");
			return new LocationData()
			{
				Country = GetFromFeatures(features, "country"),
				Region = GetFromFeatures(features, "region"),
				Postcode = GetFromFeatures(features, "postcode"),
				District = GetFromFeatures(features, "district"),
				Place = GetFromFeatures(features, "place"),
				Locality = GetFromFeatures(features, "locality"),
				Neighborhood = GetFromFeatures(features, "neighborhood"),
				Address = GetFromFeatures(features, "address"),
			};
		}

		public async Task<GeoLocation> GetLocation(string searchString)
		{
			var response = await m_httpClient.GetAsync(
				"/geocoding/v5/mapbox.places/" +    // TODO: change to permanent?
				$"{searchString}.json" +
				$"?access_token={ApiKey}");
			var body = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				Logger.Error($"{nameof(MapBoxGeocodingService)}: {response.StatusCode}\n{body}");
				return default;
			}
			var json = JsonConvert.DeserializeObject<JObject>(body);
			var firstResult = json.Value<JArray>("features").FirstOrDefault();
			if(firstResult == null)
			{
				return default;
			}
			var center = firstResult.Value<JArray>("center");
			return new GeoLocation(center[1].Value<double>(), center[0].Value<double>());
		}
	}
}
