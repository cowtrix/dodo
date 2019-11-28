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
		[View(EUserPriviligeLevel.USER)]
		public string RebellionName;

		[View(EUserPriviligeLevel.USER)]
		public GeoLocation Location;

		[View(EUserPriviligeLevel.USER)]
		public string Description;

		[View(EUserPriviligeLevel.USER)]
		public List<WorkingGroup> WorkingGroups
		{
			get
			{
				return DodoServer.ResourceManager<WorkingGroup>().Get(wg => wg.IsChildOf(this)).ToList();
			}
		}

		[View(EUserPriviligeLevel.ADMIN)]
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
