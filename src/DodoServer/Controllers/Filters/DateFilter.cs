using Resources;
using System;
using Dodo;
using System.Collections.Generic;
using System.Linq;

namespace DodoResources
{
	public class DateFilter
	{
		public string startdate { get; set; }
		public string enddate { get; set; }

		private bool m_generatedData;
		private DateTime m_startDate;
		private DateTime m_endDate;
		private bool m_empty;

		public void GenerateFilterData()
		{
			if (m_generatedData)
			{
				return;
			}
			m_generatedData = true;
			m_startDate = string.IsNullOrEmpty(startdate) ? DateTime.MinValue : DateTime.Parse(startdate);
			m_endDate = string.IsNullOrEmpty(enddate) ? DateTime.MaxValue : DateTime.Parse(enddate);
			if (string.IsNullOrEmpty(startdate) && string.IsNullOrEmpty(enddate))
			{
				m_empty = true;
			}
		}

		public bool Filter(IRESTResource rsc)
		{
			GenerateFilterData();
			if (m_empty)
			{
				return true;
			}
			if (rsc is ITimeBoundResource timeboundResource)
			{
				return timeboundResource.StartDate <= m_endDate || timeboundResource.EndDate >= m_startDate;
			}
			return false;
		}

		public IEnumerable<IRESTResource> Mutate(IEnumerable<IRESTResource> rsc)
		{
			GenerateFilterData();
			if (m_empty || !rsc.Any())
			{
				return rsc;
			}
			return rsc.OrderBy(rsc => (rsc as ITimeBoundResource)?.StartDate);
		}
	}
}
