using Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;

namespace Resources.Location
{
	public static class LocationManager
	{
		private static IGeocodingService m_service;
		private static ConcurrentDictionary<GeoLocation, bool> m_pendingRequests;
		private static PersistentStore<GeoLocation, LocationData> m_locationCache;

		static LocationManager()
		{
			m_service = new MapBoxGeocodingService();
			m_pendingRequests = new ConcurrentDictionary<GeoLocation, bool>();
			m_locationCache = new PersistentStore<GeoLocation, LocationData>("geolocation", "locationcache");
		}

		public static bool Enabled => m_service != null && m_service.Enabled;

		public static void ClearCache()
		{
			m_locationCache.Clear();
		}

		public static LocationData GetLocationData(GeoLocation location)
		{
			if(m_locationCache.TryGetValue(location, out var data) && data != null)
			{
				return data;
			}
			if(m_pendingRequests.ContainsKey(location))
			{
				return null;
			}
			m_pendingRequests.TryAdd(location, true);
			m_locationCache[location] = null;
			Task lookup = new Task(async () =>
			{
				var locData = await m_service.GetLocationData(location);
				m_locationCache[location] = locData;
				m_pendingRequests.TryRemove(location, out _);
				Logger.Debug($"Set location cache for {location}: {locData}");
			});
			lookup.Start();
			return null;
		}

		public static async Task<LocationData> GetLocationDataAsync(GeoLocation location)
		{
			if (m_locationCache.TryGetValue(location, out var locData) && locData != null)
			{
				return locData;
			}
			m_pendingRequests.TryAdd(location, true);
			m_locationCache[location] = null;
			locData = await m_service.GetLocationData(location);
			m_locationCache[location] = locData;
			m_pendingRequests.TryRemove(location, out _);
			Logger.Debug($"Set location cache for {location}: {locData}");
			return locData;
		}

		public static async Task<GeoLocation> GetLocation(string searchString)
		{
			var locations = await m_service.GetLocations(searchString);
			if (locations != null)
			{
				return locations.FirstOrDefault();
			}
			return default;
		}

		public static Task<IEnumerable<GeoLocation>> GetLocations(string searchString)
		{
			return m_service.GetLocations(searchString);
		}

		internal static void ForceCache(GeoLocation loc, LocationData data)
		{
			m_locationCache[loc] = data;
		}
	}
}
