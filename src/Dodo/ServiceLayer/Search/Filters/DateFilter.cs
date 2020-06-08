using Resources;
using System;
using Dodo;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.Resources
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
			m_startDate = string.IsNullOrEmpty(StartDate) ? DateTime.MinValue : DateTime.Parse(StartDate);
			m_endDate = string.IsNullOrEmpty(EndDate) ? DateTime.MaxValue : DateTime.Parse(EndDate);
			if (string.IsNullOrEmpty(StartDate) && string.IsNullOrEmpty(EndDate))
			{
				m_empty = true;
			}
		}

		public bool Filter(IRESTResource rsc)
		{
			Initialise();
			if (m_empty)
			{
				return true;
			}
			if (rsc is ITimeBoundResource timeboundResource)
			{
				return timeboundResource.StartDate <= m_endDate && timeboundResource.EndDate >= m_startDate;
			}
			return false;
		}

		public IEnumerable<IRESTResource> Mutate(IEnumerable<IRESTResource> rsc)
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
