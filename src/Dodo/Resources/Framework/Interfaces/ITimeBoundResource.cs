using Resources;
using System;

namespace Dodo
{
	public interface ITimeBoundResource : IPublicResource
	{
		DateTime StartDate { get; }
		DateTime EndDate { get; }
	}
}
