using Common;
using Dodo.Rebellions;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.WorkingGroups
{
	public class WorkingGroupRESTHandler : GroupResourceRESTHandler<WorkingGroup>
	{
		public class CreationSchema : IRESTResourceSchema
		{
			public CreationSchema(string name)
			{
				Name = name;
			}
			public string Name;
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
			if (!url.EndsWith(WorkingGroup.ROOT))
			{
				return false;
			}
			url = url.Substring(0, url.Length - WorkingGroup.ROOT.Length);
			GroupResource group = GetParentFromURL(url);
			if(group == null)
			{
				return false;
			}
			return true;
		}

		protected override string CreationPostfix => $"/{WorkingGroup.ROOT}/create";

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema("");
		}

		protected override WorkingGroup CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var user = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			if(user == null)
			{
				throw HTTPException.LOGIN;
			}
			GroupResource group = GetParentFromURL(request.Url);
			if (group == null)
			{
				throw new HTTPException("Valid parent doesn't exist at " + request.Url, 404);
			}
			if (!ValidationExtensions.NameIsValid(info.Name, out var error))
			{
				throw new Exception(error);
			}
			var newWorkingGroup = new WorkingGroup(user, passphrase, group, info);
			if (URLIsCreation(newWorkingGroup.ResourceURL))
			{
				throw new Exception("Reserved Resource URL");
			}
			ResourceManager.Add(newWorkingGroup);
			return newWorkingGroup;
		}
	}
}