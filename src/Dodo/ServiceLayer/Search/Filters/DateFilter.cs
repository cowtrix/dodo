using Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.DodoResources
{
	public class DateFilter : ISearchFilter
	{
		public string StartDate { get; set; }
		public string EndDate { get; set; }

		private bool m_generatedData;
		private DateTime m_startDate;
		private DateTime m_endDate;
		private bool m_empty;

		public void Initialise()
		{
			if (m_generatedData)
			{
				return;
			}
			m_generatedData = true;
			m_startDate = string.IsNullOrEmpty(StartDate) ? DateTime.UtcNow : DateTime.Parse(StartDate);
			m_endDate = string.IsNullOrEmpty(EndDate) ? DateTime.MaxValue : DateTime.Parse(EndDate);
			m_empty = string.IsNullOrEmpty(StartDate) && string.IsNullOrEmpty(EndDate);
		}

		public bool Filter(IPublicResource rsc)
		{
			Initialise();
			if (rsc is ITimeBoundResource timeboundResource)
			{
				return timeboundResource.StartDate <= m_endDate && timeboundResource.EndDate >= m_startDate;
			}
			return true;
		}

		public IEnumerable<IPublicResource> Mutate(IEnumerable<IPublicResource> rsc)
		{
			Initialise();
			if (m_empty || !rsc.Any())
			{
				return rsc;
			}
			return rsc.OrderBy(rsc => (rsc as ITimeBoundResource)?.StartDate);
		}
	}
}
