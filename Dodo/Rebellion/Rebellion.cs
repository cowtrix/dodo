using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodo.Rebellion
{
	public struct GeoLocation
	{
		public double Latitude;
		public double Longitude;
	}

	public class Rebellion
	{
		public GeoLocation Location;
		public ConcurrentDictionary<string, WorkingGroup> WorkingGroups = new ConcurrentDictionary<string, WorkingGroup>();
		public ConcurrentDictionary<string, LocalGroup> LocalGroups = new ConcurrentDictionary<string, LocalGroup>();
	}

	public class WorkingGroup
	{
	}

	public class LocalGroup
	{
	}
}
