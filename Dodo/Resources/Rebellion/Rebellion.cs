using Common;
using Common.Extensions;
using Common.Security;
using Dodo.Gateways;
using Dodo.LocalGroups;
using Dodo.Sites;
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
	[Name("Rebellion")]
	public class Rebellion : GroupResource
	{
		public const string ROOT = "rebellions";

		[View(EPermissionLevel.USER)]
		public GeoLocation Location;

		[View(EPermissionLevel.USER)]
		public string Description;

		[View(EPermissionLevel.USER)]
		public List<string> WorkingGroups
		{
			get
			{
				return ResourceUtility.GetManager<WorkingGroup>().Get(wg => wg.IsChildOf(this)).Select(x => x.GUID.ToString()).ToList();
			}
		}

		[View(EPermissionLevel.ADMIN)]
		public RebellionBotConfiguration BotConfiguration = new RebellionBotConfiguration();

		public Rebellion() : base() { }

		public Rebellion(User creator, Passphrase passphrase, RebellionRESTHandler.CreationSchema schema) : base(creator, passphrase, schema.Name, null)
		{
			Location = schema.Location;
		}

		public override string ResourceURL
		{
			get
			{
				return $"{ROOT}/{Name.StripForURL()}";
			}
		}

		public override bool CanContain(Type type)
		{
			if(type == typeof(WorkingGroup) || type == typeof(Site))
			{
				return true;
			}
			return false;
		}
	}
}
