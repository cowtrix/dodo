using Common;
using Resources;
using System;

namespace Dodo
{
	public interface ILocationalResource : IRESTResource
	{
		GeoLocation Location { get; }
	}

	public interface ITimeBoundResource : IRESTResource
	{
		DateTime StartDate { get; }
		DateTime EndDate { get; }
	}
}
