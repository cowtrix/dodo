using Resources;
using System.Collections.Generic;

namespace Dodo.Resources
{
	public interface ISearchFilter
	{
		void Initialise();
		bool Filter(IRESTResource rsc);
		IEnumerable<IRESTResource> Mutate(IEnumerable<IRESTResource> rsc);
	}
}
