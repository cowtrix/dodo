using Common.Extensions;
using GeoCoordinatePortable;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Resources
{
	public static class GeocodingService
	{
		//private PersistentStore<>
	}

	/// <summary>
	/// Represents a geographic location on the Earth's surface
	/// </summary>
	public struct GeoLocation : IVerifiable
	{
		public GeoCoordinatePortable.GeoCoordinate ToCoordinate() => new GeoCoordinate(Latitude, Longitude);
		[JsonProperty]
		private Guid m_reverseGeocodingKey { get; set; }
		[JsonProperty]
		[Range(-180, 180)]
		public double Latitude { get; set; }
		[JsonProperty]
		[Range(-90, 90)]
		public double Longitude { get; set; }

		public GeoLocation(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		public bool CanVerify()
		{
			return true;
		}

		public override bool Equals(object obj)
		{
			return obj is GeoLocation location &&
				   Latitude == location.Latitude &&
				   Longitude == location.Longitude;
		}

		public override int GetHashCode()
		{
			var hashCode = -1416534245;
			hashCode = hashCode * -1521134295 + Latitude.GetHashCode();
			hashCode = hashCode * -1521134295 + Longitude.GetHashCode();
			return hashCode;
		}

		public static bool operator ==(GeoLocation left, GeoLocation right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(GeoLocation left, GeoLocation right)
		{
			return !(left == right);
		}
	}
}
