using Common;
using Common.Config;
using GeoTimeZone;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Resources.Location
{
	public class MapBoxGeocodingService : IGeocodingService
	{
		static Uri m_baseUrl = new Uri("https://api.mapbox.com");
		static string ApiKey = ConfigManager.GetValue($"{nameof(MapBoxGeocodingService)}_{nameof(ApiKey)}", "");
		public bool Enabled => !string.IsNullOrEmpty(ApiKey);
		static HttpClient m_httpClient;

		static MapBoxGeocodingService()
		{
			m_httpClient = new HttpClient() { BaseAddress = m_baseUrl };
			Logger.Info($"Using MapBox API Key {ApiKey}");
		}

		public async Task<LocationData> GetLocationData(GeoLocation location)
		{
			string GetFromFeatures(JArray arr, string key)
			{
				return arr.FirstOrDefault(j => j.Value<JArray>("place_type").Values<string>().Contains(key))?.Value<string>("text");
			}
			if (!Enabled)
			{
				Logger.Warning($"No MapBox API Key is set.");
				return null;
			}
			//Logger.Debug($"Executing MapBox API query for {location}");
			var response = await m_httpClient.GetAsync(
				"/geocoding/v5/mapbox.places/" +    // TODO: change to permanent?
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
			var timezone = TimeZoneLookup.GetTimeZone(location.Latitude, location.Longitude);
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
				TimezoneID = timezone.Result,
			};
		}

		public async Task<IEnumerable<GeoLocation>> GetLocations(string searchString)
		{
			if (!Enabled)
			{
				Logger.Warning($"No MapBox API Key is set.");
				return null;
			}
			void SetViaString(LocationData data, string name, JToken context)
			{
				switch (name)
				{
					case "address":
						data.Address = context["text"].Value<string>();
						break;
					case "neighborhood":
						data.Neighborhood = context["text"].Value<string>();
						break;
					case "locality":
						data.Locality = context["text"].Value<string>();
						break;
					case "place":
						data.Place = context["text"].Value<string>();
						break;
					case "district":
						data.District = context["text"].Value<string>();
						break;
					case "postcode":
						data.Postcode = context["text"].Value<string>();
						break;
					case "region":
						data.Region = context["text"].Value<string>();
						break;
					case "country":
						data.Country = context["text"].Value<string>();
						break;
					default:
						Logger.Error($"Unexpected context type {name}");
						break;
				}
			}
			var response = await m_httpClient.GetAsync(
				"/geocoding/v5/mapbox.places/" +    // TODO: change to permanent?
				$"{Uri.EscapeUriString(searchString)}.json" +
				$"?access_token={ApiKey}");
			var body = await response.Content.ReadAsStringAsync();
			if (!response.IsSuccessStatusCode)
			{
				Logger.Error($"{response.StatusCode}\n{body}");
				return null;
			}
			var json = JsonConvert.DeserializeObject<JObject>(body);
			List<GeoLocation> results = new List<GeoLocation>();
			foreach (var result in json.Value<JArray>("features").Values<JObject>())
			{
				var center = result.Value<JArray>("center");
				var loc = new GeoLocation(center[1].Value<double>(), center[0].Value<double>());
				var data = new LocationData();
				foreach (var context in result.Value<JArray>("context"))
				{
					var contextType = context["id"].Value<string>();
					contextType = contextType.Substring(0, contextType.IndexOf('.'));
					SetViaString(data, contextType, context);
				}
				LocationManager.ForceCache(loc, data);
				results.Add(loc);
			}
			return results;
		}

	}
}
