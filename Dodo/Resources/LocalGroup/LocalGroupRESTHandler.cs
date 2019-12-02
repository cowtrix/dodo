using Common;
using Common.Extensions;
using Dodo.Resources;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dodo.LocalGroups
{
	public class LocalGroupRESTHandler : GroupResourceRESTHandler<LocalGroup>
	{
		public class CreationSchema : IRESTResourceSchema
		{
			public string Name = "";
			public GeoLocation Location = new GeoLocation();
			public string Description = "";

			public CreationSchema(string name, GeoLocation location, string description)
			{
				Name = name;
				Location = location;
				Description = description;
			}
		}

		public override void AddRoutes(List<Route> routeList)
		{
			routeList.Add(new Route(
				$"{GetType().Name} LIST",
				EHTTPRequestType.GET,
				URLIsList,
				WrapRawCall((req) => HttpBuilder.OK(ResourceManager.Get(x => true)
					.Select(x => x.GUID.ToString())
					.ToList()))
				));
			base.AddRoutes(routeList);
		}

		protected bool URLIsList(string url)
		{
			return url == LocalGroup.ROOT;
		}

		protected override bool URLIsCreation(string url)
		{
			return url == LocalGroup.ROOT + "/create";
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema("Name of local group", new GeoLocation(), "Public description of local group");
		}

		protected override LocalGroup CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var user = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			if(user == null)
			{
				throw HttpException.LOGIN;
			}
			if(!ValidationExtensions.NameIsValid(info.Name, out var error))
			{
				throw new Exception(error);
			}
			var localGroup = new LocalGroup(user, passphrase, info);
			if(URLIsCreation(localGroup.ResourceURL))
			{
				throw new Exception("Reserved Resource URL");
			}
			localGroup.Verify();
			return localGroup;
		}
	}
}
