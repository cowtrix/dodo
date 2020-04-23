using Resources;
using System;
using System.Collections.Generic;

namespace Dodo.Resources
{
	public class ParentFilter : ISearchFilter
	{
		public Guid Parent { get; set; }

		public bool Filter(IRESTResource rsc)
		{
			if(Parent == default)
			{
				return true;
			}
			if(rsc is IOwnedResource owned)
			{
				return owned.Parent.Guid == Parent;
			}
			return false;
		}

		public void Initialise()
		{
		}

		public IEnumerable<IRESTResource> Mutate(IEnumerable<IRESTResource> rsc)
		{
			return rsc;
		}
	}
}
