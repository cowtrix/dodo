﻿using Common;
using Common.Extensions;
using Dodo.Rebellions;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.Sites
{
	public class SiteRESTHandler : DodoRESTHandler<Site>
	{
		public class CreationSchema : IRESTResourceSchema
		{
			public CreationSchema(string name, string type, GeoLocation location, string description)
			{
				Name = name;
				Type = type;
				Location = location;
			}
			public string Name = "";
			public string Type = "";
			public string Description = "";
			public GeoLocation Location = new GeoLocation();
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
			if (!url.EndsWith(Site.ROOT))
			{
				return false;
			}
			url = url.Substring(0, url.Length - Site.ROOT.Length);
			GroupResource group = GetParentFromURL(url);
			if(group == null)
			{
				return false;
			}
			return true;
		}

		protected override string CreationPostfix => $"/{Site.ROOT}/create";

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema("The name of the site",
				$"The type of site: {typeof(OccupationalSite).FullName}, {typeof(Sanctuary).FullName}, {typeof(March).FullName}, {typeof(ActionSite).FullName}",
				new GeoLocation(), "A public description of this site");
		}

		protected override Site CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var user = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			if(user == null)
			{
				throw HttpException.LOGIN;
			}
			Rebellion rebellion = GetParentFromURL(request.Url) as Rebellion;
			if (rebellion == null)
			{
				throw new HttpException("Valid parent doesn't exist at " + request.Url, 404);
			}
			if (!ValidationExtensions.NameIsValid(info.Name, out var error))
			{
				throw new Exception(error);
			}
			var type = Type.GetType(info.Type);
			if(!typeof(Site).IsAssignableFrom(type))
			{
				throw new HttpException("Bad Request", 400);
			}
			var newSite = Activator.CreateInstance(type, user, passphrase, rebellion, info) as Site;
			if (URLIsCreation(newSite.ResourceURL))
			{
				throw new Exception("Reserved Resource URL");
			}
			newSite.Verify();
			return newSite;
		}
	}
}