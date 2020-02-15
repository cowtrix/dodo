using Common;
using Resources;
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
