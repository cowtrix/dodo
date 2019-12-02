using Common;
using Dodo.Users;
using Dodo.Rebellions;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Collections.Concurrent;
using System;
using Dodo.Roles;
using Common.Extensions;
using Common.Security;
using Newtonsoft.Json;

namespace Dodo.LocalGroups
{
	public class LocalGroup : GroupResource
	{
		public const string ROOT = "localgroups";

		public LocalGroup() : base() { }

		public LocalGroup(User owner, Passphrase passphrase, LocalGroupRESTHandler.CreationSchema schema) : base(owner, passphrase, schema.Name, null)
		{
			Location = schema.Location;
			Description = schema.Description;
		}

		public override string ResourceURL => $"{ROOT}/{Name.StripForURL()}";

		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; private set; }

		[View(EPermissionLevel.PUBLIC)]
		[JsonProperty]
		public string Description { get; private set; }

		public override bool CanContain(Type type)
		{
			if (type == typeof(Role))
			{
				return true;
			}
			return false;
		}
	}
}
