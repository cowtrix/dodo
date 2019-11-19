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
		public HttpResponse Create(HttpRequest request)
		{
			return CreateObject(request);
		}

		[Route("List all working groups", "^workinggroups$", EHTTPRequestType.GET)]
		public HttpResponse List(HttpRequest request)
		{
			var owner = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			return HttpBuilder.OK(DodoServer.ResourceManager<WorkingGroup>().Get(x => true).ToList()
				.GenerateJsonView(EPermissionLevel.USER, owner, passphrase));
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
			return new { RebellionGUID = "", ParentGroup = "", WorkingGroupName = "" };
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
			string parentWGstring = info.ParentGroup.ToString();
			ResourceReference<WorkingGroup> parentWG = default;
			if (!string.IsNullOrEmpty(parentWGstring))
			{
				if(!Guid.TryParse(parentWGstring, out Guid guid))
				{
					throw new HTTPException($"Working Group with Guid {parentWGstring} not found", 404);
				}
				parentWG = rebellion.WorkingGroups.Single(x => x.Guid == guid);
				parentWG.CheckValue();
			}
			var newWorkingGroup = new WorkingGroup(user, rebellion, parentWG.Value, info.WorkingGroupName.ToString());
			DodoServer.ResourceManager<WorkingGroup>().Add(newWorkingGroup);
			return newWorkingGroup;
		}

		protected override void DeleteObjectInternal(WorkingGroup target)
		{
			DodoServer.ResourceManager<WorkingGroup>().Delete(target);
		}
	}
}
