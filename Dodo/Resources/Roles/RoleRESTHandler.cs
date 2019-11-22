using Common;
using Dodo.Rebellions;
using Dodo.Resources;
using Dodo.Users;
using Dodo.WorkingGroups;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dodo.Roles
{
	public class RoleRESTHandler : DodoRESTHandler<Role>
	{
		protected override string CreationPostfix => Role.ROOT + "/create";

		public class CreationSchema : IRESTResourceSchema
		{
			public string Name = "";
			public string Mandate = "";
		}

		[Route("List all roles", "^roles$", EHTTPRequestType.GET)]
		public static HttpResponse List(HttpRequest request)
		{
			var owner = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			return HttpBuilder.OK(DodoServer.ResourceManager<WorkingGroup>().Get(x => true).ToList()
				.GenerateJsonView(EPermissionLevel.USER, owner, passphrase));
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema() { Name = "Test Working Group" };
		}

		protected override Role CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var user = DodoRESTServer.GetRequestOwner(request);
			if(user == null)
			{
				throw HTTPException.LOGIN;
			}
			var group = GetParentFromURL(request.Url);
			if(group == null)
			{
				throw new HTTPException("Valid parent doesn't exist at " + request.Url, 404);
			}
			var newRole = new Role(group, info.Name, info.Mandate);
			if (URLIsCreation(newRole.ResourceURL))
			{
				throw new Exception("Reserved Resource URL");
			}
			DodoServer.ResourceManager<Role>().Add(newRole);
			return newRole;
		}
	}
}
