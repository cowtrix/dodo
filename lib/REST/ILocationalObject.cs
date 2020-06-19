using Common;
using Resources;
using Resources.Location;

namespace Resources
{
	public interface ILocationalResource : IRESTResource
	{
		GeoLocation Location { get; }
	}
}
