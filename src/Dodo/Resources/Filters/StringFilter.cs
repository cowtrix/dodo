using Resources;
using System;
using Dodo;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.Resources
{
	public class StringFilter : ISearchFilter
	{
		public string Search { get; set; }

		Dictionary<Guid, int> m_hitCount = new Dictionary<Guid, int>();

		public bool Filter(IRESTResource rsc)
		{
			if (string.IsNullOrEmpty(Search))
			{
				return true;
			}
			var view = DodoJsonViewUtility.GenerateJsonView(rsc, EPermissionLevel.PUBLIC, null, default);
			int count = 0;
			FindCount(view, ref count, Search);
			if (count > 0)
			{
				m_hitCount[rsc.Guid] = count;
				return true;
			}
			return false;
		}

		public IEnumerable<IRESTResource> Mutate(IEnumerable<IRESTResource> rsc)
		{
			if (string.IsNullOrEmpty(Search) || !rsc.Any())
			{
				return rsc;
			}
			return rsc.OrderBy(r =>
			{
				m_hitCount.TryGetValue(r.Guid, out var hitCount);
				return hitCount;
			});
		}

		static void FindCount(Dictionary<string, object> values, ref int count, string searchValue)
		{
			foreach (var val in values)
			{
				if (val.Value is Dictionary<string, object> subDict)
				{
					FindCount(subDict, ref count, searchValue);
				}
				else if (val.Value != null && val.Value.ToString().ToLowerInvariant()
					.Contains(searchValue.ToLowerInvariant()))
				{
					count++;
				}
			}
		}

		public void Initialise()
		{
		}
	}
}
