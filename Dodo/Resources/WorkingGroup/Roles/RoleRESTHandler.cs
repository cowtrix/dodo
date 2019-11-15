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
		const string URL_REGEX = Role.ROOT + "/(?:^/)*";

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

		[Route("Get a role", URL_REGEX, EHTTPRequestType.GET)]
		public HttpResponse GetUser(HttpRequest request)
		{
			var owner = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			var workingGroup = GetResource(request.Url);
			if (workingGroup == null)
			{
				throw HTTPException.NOT_FOUND;
			}
			if (!workingGroup.IsAuthorised(owner, request, out var view))
			{
				throw HTTPException.FORBIDDEN;
			}
			return HttpBuilder.OK(workingGroup.GenerateJsonView(view, owner, passphrase));
		}

		[Route("Delete a role", URL_REGEX, EHTTPRequestType.DELETE)]
		public HttpResponse DeleteUser(HttpRequest request)
		{
			return DeleteObject(request);
		}

		[Route("Update a role", URL_REGEX, EHTTPRequestType.PATCH)]
		public HttpResponse UpdateUser(HttpRequest request)
		{
			return UpdateObject(request);
		}

		protected override Role GetResource(string url)
		{
			if(!Regex.IsMatch(url, URL_REGEX))
			{
				return null;
			}
			return DodoServer.ResourceManager<Role>().GetSingle(x => x.ResourceURL == url);
		}

		protected override dynamic GetCreationSchema()
		{
			return new { GroupGUID = "", Name = "", Mandate = "" };
		}

		protected override Role CreateFromSchema(HttpRequest request, dynamic info)
		{
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
			var newRole = new Role(group, info.Name.ToString(), info.Mandate.ToString());
			DodoServer.ResourceManager<Role>().Add(newRole);
			return newRole;
		}

		protected override void DeleteObjectInternal(Role target)
		{
			DodoServer.ResourceManager<Role>().Delete(target);
		}
	}
}
