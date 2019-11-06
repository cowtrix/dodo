using Common;
using Dodo.LocalGroups;
using Dodo.Users;
using Dodo.WorkingGroups;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Dodo.Rebellions
{
	public class Rebellion : DodoResource
	{
		public const string ROOT = "rebellions";
		[View(EViewVisibility.PUBLIC)]
		[NoPatch]
		public string RebellionName;
		[View(EViewVisibility.PUBLIC)]
		public GeoLocation Location;
		[View(EViewVisibility.PUBLIC)]
		public ConcurrentBag<ResourceReference<WorkingGroup>> WorkingGroups = new ConcurrentBag<ResourceReference<WorkingGroup>>();
		[View(EViewVisibility.PUBLIC)]
		public ConcurrentBag<ResourceReference<LocalGroup>> LocalGroups = new ConcurrentBag<ResourceReference<LocalGroup>>();

		public Rebellion (User creator, string rebellionName, GeoLocation location) : base(creator)
		{
			RebellionName = rebellionName;
			Location = location;
		}

		public override string ResourceURL
		{
			get
			{
				return $"{ROOT}/{RebellionName.StripForURL()}";
			}
		}

		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EViewVisibility visibility)
		{
			if(requestOwner == Creator.Value)
			{
				visibility = EViewVisibility.OWNER;
				return true;
			}
			visibility = EViewVisibility.PUBLIC;
			return true;
		}
	}

	public abstract class RebellionResource : DodoResource
	{
		public Rebellion Rebellion { get; private set; }
		public RebellionResource(User creator, Rebellion owner) : base(creator)
		{
			Rebellion = owner;
		}
		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EViewVisibility visibility)
		{
			return Rebellion.IsAuthorised(requestOwner, request, out visibility);
		}
	}
}
