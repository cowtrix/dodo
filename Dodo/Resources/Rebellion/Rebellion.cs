using Common;
using Dodo.Users;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Dodo.Rebellions
{
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
		public ConcurrentBag<ResourceReference<WorkingGroup>> WorkingGroups = new ConcurrentBag<ResourceReference<WorkingGroup>>();
		[View]
		public ConcurrentBag<ResourceReference<LocalGroup>> LocalGroups = new ConcurrentBag<ResourceReference<LocalGroup>>();

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
				return $"rebellions/{RebellionName.StripForURL()}";
			}
		}
	}

	public abstract class RebellionResource : Resource
	{
		public Rebellion Rebellion { get; private set; }
		public RebellionResource(Rebellion owner)
		{
			Rebellion = owner;
		}
	}
}
