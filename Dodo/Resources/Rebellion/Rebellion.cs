using Common;
using Dodo.Users;
using SimpleHttpServer.REST;
using System;
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
		[View]
		public string RebellionName;
		[View]
		[NoPatch]
		public Guid Creator;
		[View]
		public GeoLocation Location;
		[View]
		public ConcurrentDictionary<string, WorkingGroup> WorkingGroups = new ConcurrentDictionary<string, WorkingGroup>();
		[View]
		public ConcurrentDictionary<string, LocalGroup> LocalGroups = new ConcurrentDictionary<string, LocalGroup>();

		public Rebellion (User creator, string rebellionName, GeoLocation location)
		{
			Creator = creator.UUID;
			RebellionName = rebellionName;
			Location = location;
		}

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
