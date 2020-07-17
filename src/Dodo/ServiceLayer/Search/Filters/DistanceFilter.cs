using Common.Extensions;
using Resources;
using System.Linq;
using System;
using Dodo;
using GeoCoordinatePortable;
using System.Collections.Generic;
using Dodo.Rebellions;
using Dodo.LocalGroups;
using Dodo.LocationResources;
using Common.Config;
using Common;
using System.Reflection;

namespace Dodo.DodoResources
{
	public class DistanceFilter : ISearchFilter
	{
		private int TransitionDistance => ConfigManager.GetValue($"{nameof(DistanceFilter)}_{nameof(TransitionDistance)}", 100);

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
			return true;
		}

		public IEnumerable<IRESTResource> Mutate(IEnumerable<IRESTResource> rsc)
		{
			Initialise();
			if (!rsc.Any() || m_empty)
			{
				return rsc;
			}
			if (Distance < TransitionDistance)
			{
				return rsc.OrderBy(GetDist);
			}
			else
			{
				return rsc.OrderBy(rsc => rsc.GetType().GetCustomAttribute<SearchPriority>()?.Priority)
					.ThenBy(GetDist);
			}
		}

		private double? GetDist(IRESTResource rsc)
		{
			if(rsc is ILocationalResource loc)
			{
				return loc.Location.ToCoordinate().GetDistanceTo(m_coordinate);
			}
			if(rsc is IOwnedResource owned)
			{
				return owned.Parent.GetValue<ILocationalResource>()?.Location.ToCoordinate().GetDistanceTo(m_coordinate);
			}
			return null;
		}
	}
}
