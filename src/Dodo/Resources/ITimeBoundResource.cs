using Resources;
using System;

namespace Dodo
{
	public interface ITimeBoundResource : IRESTResource
	{
		DateTime StartDate { get; }
		DateTime EndDate { get; }
	}
}
