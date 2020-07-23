using Resources;
using System;
using System.Collections.Generic;

namespace Dodo.DodoResources
{
	public class ParentFilter : ISearchFilter
	{
		public string Parent { get; set; }

		public bool Filter(IRESTResource rsc)
		{
			if(string.IsNullOrEmpty(Parent))
			{
				return true;
			}
			if(rsc is IOwnedResource owned)
			{
				return owned.Parent.Slug == Parent || owned.Parent.Guid.ToString() == Parent;
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
