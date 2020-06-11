using Common;
using Common.Extensions;
using GeoCoordinatePortable;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Resources.Location
{
	/// <summary>
	/// Represents a geographic location on the Earth's surface
	/// </summary>
	[Serializable]
	public class GeoLocation : IVerifiable
	{
		[BsonIgnore]
		[Name("Address")]
		[View]
		public LocationData LocationData
		{
			get
			{
				if(m_data == null)
				{
					m_data = LocationManager.GetLocationData(this);
				}
				return m_data;
			}
		}

		[JsonIgnore]
		[BsonElement]
		private LocationData m_data;

		public GeoCoordinatePortable.GeoCoordinate ToCoordinate() => new GeoCoordinate(Latitude, Longitude);
		[JsonProperty]
		[Range(-90, 90)]
		[View]
		public double Latitude { get { return m_lat; } set { m_lat = WrapClamp(value, -90, 90); } }
		private double m_lat;

		[JsonProperty]
		[Range(-180, 180)]
		[View]
		public double Longitude { get { return m_long; } set { m_long = WrapClamp(value, -180, 180); } }
		private double m_long;

		public GeoLocation(double latitude, double longitude)
		{
			m_lat = WrapClamp(latitude, -90, 90);
			m_long = WrapClamp(longitude, -180, 180);
			m_data = null;
		}

		private static double WrapClamp(double x, double x_min, double x_max)
		{
			return (((x - x_min) % (x_max - x_min)) + (x_max - x_min)) % (x_max - x_min) + x_min;
		}

		public GeoLocation()
		{
		}

		public GeoLocation(GeoLocation location) : 
			this(location.Latitude, location.Longitude)
		{
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

		public GeoLocation Offset(double latitude, double longitude)
		{
			return new GeoLocation(Latitude + latitude, Longitude + longitude);
		}

		public override string ToString()
		{
			return $"Lat:{Latitude} Long:{Longitude}" + (LocationData != null ? $" {LocationData}" : "");
		}

		public bool VerifyExplicit(out string error)
		{
			error = null;
			return true;
		}
	}
}
