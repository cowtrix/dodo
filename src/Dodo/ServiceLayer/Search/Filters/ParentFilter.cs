using Resources;
using System.Collections.Generic;

namespace Dodo.DodoResources
{
	public class ParentFilter : ISearchFilter
	{
		public string Parent { get; set; }

		public bool Filter(IPublicResource rsc)
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

		public IEnumerable<IPublicResource> Mutate(IEnumerable<IPublicResource> rsc)
		{
			return rsc;
		}
	}
}
