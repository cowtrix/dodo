using Resources;
using System.Collections.Generic;

namespace Dodo.DodoResources
{
	public interface ISearchFilter
	{
		void Initialise();
		bool Filter(IPublicResource rsc);
		IEnumerable<IPublicResource> Mutate(IEnumerable<IPublicResource> rsc);
	}
}
