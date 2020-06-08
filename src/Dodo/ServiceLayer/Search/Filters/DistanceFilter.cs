using Common.Extensions;
using Resources;
using System.Linq;
using System;
using Dodo;
using GeoCoordinatePortable;
using System.Collections.Generic;

namespace Dodo.Resources
{
	public class DistanceFilter : ISearchFilter
	{
		public string LatLong { get; set; }
		public double? Distance { get; set; }

		private bool m_generatedData;
		private GeoCoordinate m_coordinate;
		private bool m_empty;

		public void Initialise()
		{
			if (m_generatedData)
			{
				return;
			}
			if (string.IsNullOrEmpty(LatLong) && !Distance.HasValue)
			{
				m_empty = true;
				return;
			}
			if (!Distance.HasValue)
			{
				throw new Exception("distance - Parameter for distance in km is required");
			}
			if (string.IsNullOrEmpty(LatLong))
			{
				throw new Exception("latlong - Parameter for latitude and longitude, seperated by a '+' character, is required");
			}
			m_generatedData = true;
			m_coordinate = LatLong.Split(' ').Select(x => double.Parse(x))
						.Transpose(x => new GeoCoordinate(x.ElementAt(0), x.ElementAt(1)));
		}

		public bool Filter(IRESTResource rsc)
		{
			Initialise();
			if (m_empty)
			{
				return true;
			}
			if (rsc is ILocationalResource locationalResource)
			{
				return locationalResource.Location.ToCoordinate().GetDistanceTo(m_coordinate) < Distance * 1000;
			}
			return false;
		}

		public IEnumerable<IRESTResource> Mutate(IEnumerable<IRESTResource> rsc)
		{
			Initialise();
			if (!rsc.Any() || !(rsc.Any(r => r is ILocationalResource)) || m_coordinate == null)
			{
				return rsc;
			}
			if (null == m_coordinate) return rsc;
			return rsc.OrderBy(rsc => (rsc as ILocationalResource)?.Location.ToCoordinate().GetDistanceTo(m_coordinate));
		}
	}
}
