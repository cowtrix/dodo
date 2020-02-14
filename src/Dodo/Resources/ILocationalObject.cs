using Common;
using System;

namespace Dodo
{
	public interface ILocationalResource
	{
		GeoLocation Location { get; }
	}

	public interface ITimeBoundResource
	{
		DateTime StartDate { get; }
		DateTime EndDate { get; }
	}
}
