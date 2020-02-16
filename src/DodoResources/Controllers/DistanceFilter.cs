using Common.Extensions;
using Resources;
using System.Linq;
using System;
using Dodo;
using GeoCoordinatePortable;
using System.Collections.Generic;

namespace DodoResources
{
	public class DistanceFilter
	{
		public string latlong { get; set; }
		public double? distance { get; set; }

		private bool m_generatedData;
		private GeoCoordinate m_coordinate;
		private bool m_empty;

		public void GenerateFilterData()
		{
			if (m_generatedData)
			{
				return;
			}
			if (string.IsNullOrEmpty(latlong) && !distance.HasValue)
			{
				m_empty = true;
				return;
			}
			if(!distance.HasValue)
			{
				throw new Exception("dist - Parameter for distance in km is required");
			}
			if(string.IsNullOrEmpty(latlong))
			{
				throw new Exception("latlong - Parameter for latitude and longitude, seperated by a '+' character, is required");
			}
			m_generatedData = true;
			m_coordinate = latlong.Split(' ').Select(x => double.Parse(x))
						.Transpose(x => new GeoCoordinate(x.ElementAt(0), x.ElementAt(1)));
		}

		public bool Filter(IRESTResource rsc)
		{
			GenerateFilterData();
			if (m_empty)
			{
				return true;
			}
			if (rsc is ILocationalResource locationalResource)
			{
				return locationalResource.Location.ToCoordinate().GetDistanceTo(m_coordinate) < distance * 1000;
			}
			else if (!string.IsNullOrEmpty(latlong) || distance.HasValue)
			{
				throw new Exception("Invalid filter. You cannot filter this resource by location");
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
			else if (string.IsNullOrEmpty(latlong) || !distance.HasValue)
			{
				throw new Exception("Invalid filter. You cannot filter this resource by location");
			}
			return rsc.OrderBy(rsc => (rsc as ILocationalResource)?.Location.ToCoordinate().GetDistanceTo(m_coordinate));
		}
	}
}
