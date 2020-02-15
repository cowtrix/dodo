using Common.Extensions;
using GeoCoordinatePortable;
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
		[JsonProperty]
		public GeoCoordinatePortable.GeoCoordinate Coordinate { get; private set; }
		[JsonProperty]
		private Guid m_reverseGeocodingKey { get; set; }

		public GeoLocation(double latitude, double longitude)
		{
			Coordinate = new GeoCoordinatePortable.GeoCoordinate(latitude, longitude);
		}


		public bool CanVerify()
		{
			return true;
		}

		public override bool Equals(object obj)
		{
			return obj is GeoLocation location &&
				   EqualityComparer<GeoCoordinate>.Default.Equals(Coordinate, location.Coordinate) &&
				   m_reverseGeocodingKey.Equals(location.m_reverseGeocodingKey);
		}

		public override int GetHashCode()
		{
			var hashCode = 205427882;
			hashCode = hashCode * -1521134295 + EqualityComparer<GeoCoordinate>.Default.GetHashCode(Coordinate);
			hashCode = hashCode * -1521134295 + m_reverseGeocodingKey.GetHashCode();
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
