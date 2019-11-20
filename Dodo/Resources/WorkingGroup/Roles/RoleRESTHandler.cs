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
		public class CreationSchema : IRESTResourceSchema
		{
			public string Name = "";
			public string Mandate = "";
			public string GroupGUID = "";
		}

		[Route("Create a new role", "^newrole$", EHTTPRequestType.POST)]
		public HttpResponse Register(HttpRequest request)
		{
			return CreateObject(request);
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
			return new CreationSchema();
		}

		protected override Role CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var user = DodoRESTServer.GetRequestOwner(request);
			if(user == null)
			{
				throw HTTPException.LOGIN;
			}
			var groupGuid = Guid.Parse((string)info.GroupGUID);
			var rm = ResourceUtility.GetManagerForResource(groupGuid);
			var group = (GroupResource)rm.GetSingle(x => x.GUID == groupGuid);
			if(group == null)
			{
				throw new HTTPException("Group doesn't exist with that GUID", 404);
			}
			var newRole = new Role(group, info.Name, info.Mandate);
			DodoServer.ResourceManager<Role>().Add(newRole);
			return newRole;
		}
	}
}
