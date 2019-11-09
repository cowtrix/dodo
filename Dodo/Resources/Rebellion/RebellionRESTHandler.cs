using Common;
using Dodo.Resources;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dodo.Rebellions
{
	public class RebellionRESTHandler : DodoRESTHandler<Rebellion>
	{
		const string URL_REGEX = Rebellion.ROOT + "/(?:^/)*";

		[Route("Create a new rebellion", "newrebellion", EHTTPRequestType.POST)]
		public HttpResponse Register(HttpRequest request)
		{
			return CreateObject(request);
		}

		[Route("List all rebellions", "^rebellions$", EHTTPRequestType.GET)]
		public HttpResponse List(HttpRequest request)
		{
			var owner = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			return HttpBuilder.OK(DodoServer.ResourceManager<Rebellion>().Get(x => true).ToList()
				.GenerateJsonView(EViewVisibility.PUBLIC, owner, passphrase));
		}

		[Route("Get a rebellion", URL_REGEX, EHTTPRequestType.GET)]
		public HttpResponse GetUser(HttpRequest request)
		{
			var owner = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			var rebellion = GetResource(request.Url);
			if (rebellion == null)
			{
				throw HTTPException.NOT_FOUND;
			}
			if (!rebellion.IsAuthorised(owner, request, out var view))
			{
				throw HTTPException.FORBIDDEN;
			}
			return HttpBuilder.OK(rebellion.GenerateJsonView(view, owner, passphrase));
		}

		[Route("Delete a rebellion", URL_REGEX, EHTTPRequestType.DELETE)]
		public HttpResponse DeleteUser(HttpRequest request)
		{
			return DeleteObject(request);
		}

		[Route("Update a rebellion", URL_REGEX, EHTTPRequestType.PATCH)]
		public HttpResponse UpdateUser(HttpRequest request)
		{
			return UpdateObject(request);
		}

		protected override Rebellion GetResource(string url)
		{
			if(!Regex.IsMatch(url, URL_REGEX))
			{
				return null;
			}
			return DodoServer.ResourceManager<Rebellion>().GetSingle(x => x.ResourceURL == url);
		}

		protected override dynamic GetCreationSchema()
		{
			return new { RebellionName = "", Location = new GeoLocation() };
		}

		protected override Rebellion CreateFromSchema(HttpRequest request, dynamic info)
		{
			var user = DodoRESTServer.GetRequestOwner(request);
			if(user == null)
			{
				throw HTTPException.LOGIN;
			}
			var newRebellion = new Rebellion(user, info.RebellionName.ToString(), JsonConvert.DeserializeObject<GeoLocation>(info.Location.ToString()));
			DodoServer.ResourceManager<Rebellion>().Add(newRebellion);
			return newRebellion;
		}

		protected override void DeleteObjectInternal(Rebellion target)
		{
			DodoServer.ResourceManager<Rebellion>().Delete(target);
		}
	}
}
