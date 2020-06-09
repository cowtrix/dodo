using Resources;
using System.Collections.Generic;

namespace Dodo.DodoResources
{
	public interface ISearchFilter
	{
		void Initialise();
		bool Filter(IRESTResource rsc);
		IEnumerable<IRESTResource> Mutate(IEnumerable<IRESTResource> rsc);
	}
}
