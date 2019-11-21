using Common;
using Dodo.Gateways;
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

		[View(EPermissionLevel.USER)]
		[NoPatch]
		public string RebellionName;

		[View(EPermissionLevel.USER)]
		public GeoLocation Location;

		[View(EPermissionLevel.USER)]
		public string Description;

		[NoPatch]
		[View(EPermissionLevel.USER)]
		public ConcurrentBag<ResourceReference<WorkingGroup>> WorkingGroups = new ConcurrentBag<ResourceReference<WorkingGroup>>();

		[NoPatch]
		[View(EPermissionLevel.USER)]
		public ConcurrentBag<ResourceReference<LocalGroup>> LocalGroups = new ConcurrentBag<ResourceReference<LocalGroup>>();

		[View(EPermissionLevel.ADMIN)]
		public RebellionBotConfiguration BotConfiguration = new RebellionBotConfiguration();

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

		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel visibility)
		{
			if(requestOwner == Creator.Value)
			{
				visibility = EPermissionLevel.OWNER;
				return true;
			}
			visibility = EPermissionLevel.USER;
			return true;
		}
	}
}
