using Common;
using Dodo.Rebellions;
using Dodo.Resources;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dodo.WorkingGroups
{
	public class WorkingGroupRESTHandler : DodoRESTHandler<WorkingGroup>
	{
		const string URL_REGEX = WorkingGroup.ROOT + "/(?:^/)*";

		[Route("Create a new working group", "newworkinggroup", EHTTPRequestType.POST)]
		public HttpResponse Register(HttpRequest request)
		{
			return CreateObject(request);
		}

		[Route("List all working groups", "^workinggroups$", EHTTPRequestType.GET)]
		public HttpResponse List(HttpRequest request)
		{
			var owner = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			return HttpBuilder.OK(DodoServer.ResourceManager<WorkingGroup>().Get(x => true).ToList()
				.GenerateJsonView(EViewVisibility.PUBLIC, owner, passphrase));
		}

		[Route("Get a working group", URL_REGEX, EHTTPRequestType.GET)]
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

		[Route("Delete a working group", URL_REGEX, EHTTPRequestType.DELETE)]
		public HttpResponse DeleteUser(HttpRequest request)
		{
			return DeleteObject(request);
		}

		[Route("Update a working group", URL_REGEX, EHTTPRequestType.PATCH)]
		public HttpResponse UpdateUser(HttpRequest request)
		{
			return UpdateObject(request);
		}

		protected override WorkingGroup GetResource(string url)
		{
			if(!Regex.IsMatch(url, URL_REGEX))
			{
				return null;
			}
			return DodoServer.ResourceManager<WorkingGroup>().GetSingle(x => x.ResourceURL == url);
		}

		protected override dynamic GetCreationSchema()
		{
			return new { RebellionGUID = "", WorkingGroupName = "" };
		}

		protected override WorkingGroup CreateFromSchema(HttpRequest request, dynamic info)
		{
			var user = DodoRESTServer.GetRequestOwner(request);
			if(user == null)
			{
				throw HTTPException.LOGIN;
			}
			var rebellion = DodoServer.ResourceManager<Rebellion>().GetSingle(x => x.GUID.ToString() == (string)info.RebellionGUID);
			if(rebellion == null)
			{
				throw new HTTPException("Rebellion doesn't exist with that GUID", 404);
			}
			var newWorkingGroup = new WorkingGroup(user, rebellion, info.WorkingGroupName.ToString());
			DodoServer.ResourceManager<WorkingGroup>().Add(newWorkingGroup);
			return newWorkingGroup;
		}

		protected override void DeleteObjectInternal(WorkingGroup target)
		{
			DodoServer.ResourceManager<WorkingGroup>().Delete(target);
		}
	}
}
