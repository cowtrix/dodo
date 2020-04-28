using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resources.Location
{
	public interface IGeocodingService
	{
		bool Enabled { get; }
		Task<LocationData> GetLocationData(GeoLocation location);
		Task<IEnumerable<GeoLocation>> GetLocations(string searchString);
	}
}
