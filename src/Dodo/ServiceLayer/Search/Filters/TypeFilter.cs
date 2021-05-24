using Resources;
using System.Collections.Generic;

namespace Dodo.DodoResources
{
	public class TypeFilter : ISearchFilter
	{
		public string Types { get; set; }
		private HashSet<string> m_typeArray;

		public bool Filter(IPublicResource rsc)
		{
			if (string.IsNullOrEmpty(Types))
			{
				return true;
			}
			if(m_typeArray == null)
			{
				m_typeArray = new HashSet<string>();
				var types = Types.Split(",");
				foreach(var t in types)
				{
					m_typeArray.Add(t.ToLowerInvariant());
				}
			}
			var rscType = rsc.GetType().Name.ToLowerInvariant();
			return m_typeArray.Contains(rscType);
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
