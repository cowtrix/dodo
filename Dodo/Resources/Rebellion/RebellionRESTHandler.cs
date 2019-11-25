using Common;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.Rebellions
{
	public class RebellionRESTHandler : GroupResourceRESTHandler<Rebellion>
	{
		public class CreationSchema : IRESTResourceSchema
		{
			public string Name = "";
			public GeoLocation Location = new GeoLocation();
		}

		protected override bool URLIsCreation(string url)
		{
			return url == Rebellion.ROOT + "/create";
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
			return url == Rebellion.ROOT;
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema();
		}

		protected override Rebellion CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var user = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			if(user == null)
			{
				throw HTTPException.LOGIN;
			}
			if(!ValidationExtensions.NameIsValid(info.Name, out var error))
			{
				throw new System.Exception(error);
			}
			var newRebellion = new Rebellion(user, passphrase, info);
			if (URLIsCreation(newRebellion.ResourceURL))
			{
				throw new Exception("Reserved Resource URL");
			}
			ResourceManager.Add(newRebellion);
			return newRebellion;
		}
	}
}
