using Common;
using Resources;
using Resources.Location;

namespace Dodo
{
	public interface ILocationalResource : IRESTResource
	{
		GeoLocation Location { get; }
	}
}
