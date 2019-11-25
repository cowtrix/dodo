using Newtonsoft.Json;

namespace Common
{
	/// <summary>
	/// Represents a geographic location on the Earth's surface
	/// </summary>
	public struct GeoLocation
	{
		[JsonProperty]
		public double Latitude { get; private set; }
		[JsonProperty]
		public double Longitude { get; private set; }
		public GeoLocation(double latitude, double longitude)
		{
			Latitude = latitude;
			Longitude = longitude;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is GeoLocation))
			{
				return false;
			}

			var location = (GeoLocation)obj;
			return Latitude == location.Latitude &&
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
