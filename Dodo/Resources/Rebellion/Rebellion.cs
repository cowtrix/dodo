using Common;
using Dodo.Gateways;
using Dodo.LocalGroups;
using Dodo.Users;
using Dodo.WorkingGroups;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dodo.Rebellions
{
	public class Rebellion : GroupResource
	{
		public const string ROOT = "rebellions";

		[NoPatch]
		[View(EPermissionLevel.USER)]
		public string RebellionName;

		[View(EPermissionLevel.USER)]
		public GeoLocation Location;

		[View(EPermissionLevel.USER)]
		public string Description;

		[View(EPermissionLevel.USER)]
		public List<WorkingGroup> WorkingGroups
		{
			get
			{
				return DodoServer.ResourceManager<WorkingGroup>().Get(wg => wg.IsChildOf(this)).ToList();
			}
		}

		[View(EPermissionLevel.ADMIN)]
		public RebellionBotConfiguration BotConfiguration = new RebellionBotConfiguration();

		public Rebellion(User creator, string passphrase, RebellionRESTHandler.CreationSchema schema) : base(creator, passphrase, null)
		{
			RebellionName = schema.Name;
			Location = schema.Location;
		}

		public override string ResourceURL
		{
			get
			{
				return $"{ROOT}/{RebellionName.StripForURL()}";
			}
		}

		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			if(requestOwner == Creator.Value)
			{
				permissionLevel = EPermissionLevel.OWNER;
				return true;
			}
			permissionLevel = EPermissionLevel.USER;
			return true;
		}

		public override bool CanContain(Type type)
		{
			if(type == typeof(WorkingGroup))
			{
				return true;
			}
			return false;
		}
	}
}
