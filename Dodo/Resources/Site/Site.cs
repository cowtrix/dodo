using Common;
using Common.Extensions;
using Common.Security;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.Sites
{
	public abstract class Site : DodoResource
	{
		public const string ROOT = "sites";

		[View(EPermissionLevel.USER)]
		[JsonProperty]
		public ResourceReference<Rebellion> Rebellion { get; private set; }

		[JsonProperty]
		[View(EPermissionLevel.USER)]
		public GeoLocation Location { get; private set; }

		public string Type { get { return GetType().FullName; } }

		public Site() : base() { }

		public Site(User creator, Passphrase passphrase, Rebellion rebellion, SiteRESTHandler.CreationSchema schema)
			: base(creator, schema.Name)
		{
			Rebellion = new ResourceReference<Rebellion>(rebellion);
			Location = schema.Location;
		}
		
		public override string ResourceURL => $"{Rebellion.Value.ResourceURL}/{ROOT}/{Name.StripForURL()}";

		public override bool IsAuthorised(User requestOwner, Passphrase passphrase, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			return Rebellion.Value.IsAuthorised(requestOwner, passphrase, request, out permissionLevel);
		}
	}
}