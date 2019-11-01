using SimpleHttpServer.REST;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Dodo.Rebellions
{
	public struct GeoLocation
	{
		public double Latitude;
		public double Longitude;
	}

	public class Rebellion : Resource
	{
		public string RebellionName;
		public GeoLocation Location;
		public ConcurrentDictionary<string, WorkingGroup> WorkingGroups = new ConcurrentDictionary<string, WorkingGroup>();
		public ConcurrentDictionary<string, LocalGroup> LocalGroups = new ConcurrentDictionary<string, LocalGroup>();

		public override string ResourceURL
		{
			get
			{
				return $"rebellions/{System.Net.WebUtility.UrlEncode(Regex.Replace(RebellionName.ToLower(), @"\s+", ""))}";
			}
		}
	}

	public class WorkingGroup
	{
	}

	public class LocalGroup
	{
	}
}
