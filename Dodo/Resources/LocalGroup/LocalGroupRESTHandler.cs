using Common;
using Dodo.Resources;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dodo.LocalGroups
{
	public class LocalGroupRESTHandler : DodoRESTHandler<LocalGroup>
	{
		const string CREATION_URL = "newlocalgroup";
		const string URL_REGEX = LocalGroup.ROOT + "/(?:^/)*";

		[Route("Create a new local group", "^" + CREATION_URL, EHTTPRequestType.POST)]
		public HttpResponse CreateLocalGroup(HttpRequest request)
		{
			return CreateObject(request);
		}

		[Route("List all local groups", "^localgroups$", EHTTPRequestType.GET)]
		public HttpResponse List(HttpRequest request)
		{
			var owner = DodoRESTServer.GetRequestOwner(request, out var passPhrase);
			return HttpBuilder.OK(DodoServer.ResourceManager<LocalGroup>().Get(x => true).ToList()
				.GenerateJsonView(EPermissionLevel.USER, owner, passPhrase));
		}

		protected override LocalGroup GetResource(string url)
		{
			if(!Regex.IsMatch(url, URL_REGEX))
			{
				return null;
			}
			return DodoServer.ResourceManager<LocalGroup>().GetSingle(x => x.ResourceURL == url);
		}

		protected override dynamic GetCreationSchema()
		{
			return new { Name = "", Location = new GeoLocation() };
		}

		protected override LocalGroup CreateFromSchema(HttpRequest request, dynamic info)
		{
			var user = DodoRESTServer.GetRequestOwner(request);
			if(user == null)
			{
				throw HTTPException.LOGIN;
			}
			var localGroup = new LocalGroup(user, info.Name.ToString(), JsonConvert.DeserializeObject<GeoLocation>(info.Location.ToString()));
			DodoServer.ResourceManager<LocalGroup>().Add(localGroup);
			return localGroup;
		}

		protected override void DeleteObjectInternal(LocalGroup target)
		{
			DodoServer.ResourceManager<LocalGroup>().Delete(target);
		}
	}
}
