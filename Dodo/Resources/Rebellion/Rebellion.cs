using Common;
using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Dodo.Rebellions
{
	public class Rebellion : DodoResource
	{
		[View(EViewVisibility.PUBLIC)]
		public string RebellionName;
		[View(EViewVisibility.OWNER)]
		[NoPatch]
		public ResourceReference<User> Creator;
		[View(EViewVisibility.PUBLIC)]
		public GeoLocation Location;
		[View(EViewVisibility.PUBLIC)]
		public ConcurrentBag<ResourceReference<WorkingGroup>> WorkingGroups = new ConcurrentBag<ResourceReference<WorkingGroup>>();
		[View(EViewVisibility.PUBLIC)]
		public ConcurrentBag<ResourceReference<LocalGroup>> LocalGroups = new ConcurrentBag<ResourceReference<LocalGroup>>();

		public Rebellion (User creator, string rebellionName, GeoLocation location)
		{
			Creator = new ResourceReference<User>(creator);
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

		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EViewVisibility visibility)
		{
			throw new NotImplementedException();
		}
	}

	public abstract class RebellionResource : DodoResource
	{
		public Rebellion Rebellion { get; private set; }
		public RebellionResource(Rebellion owner)
		{
			Rebellion = owner;
		}
		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EViewVisibility visibility)
		{
			return Rebellion.IsAuthorised(requestOwner, request, out visibility);
		}
	}
}
