using Common;
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
		const string URL_REGEX = "rebellions/(?:^/)*";

		[Route("Create a new rebellion", "newrebellion", EHTTPRequestType.POST)]
		public HttpResponse Register(HttpRequest request)
		{
			return CreateObject(request);
		}

		[Route("List all rebellions", "rebellions", EHTTPRequestType.GET)]
		public HttpResponse List(HttpRequest request)
		{
			return HttpBuilder.OK(DodoServer.RebellionManager.Get(x => true).ToList().GenerateJsonView());
		}

		[Route("Get a rebellion", URL_REGEX, EHTTPRequestType.GET)]
		public HttpResponse GetUser(HttpRequest request)
		{
			var user = GetResource(request.Url);
			if(user == null)
			{
				throw HTTPException.NOT_FOUND;
			}
			return HttpBuilder.OK(user.GenerateJsonView());
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
			return DodoServer.RebellionManager.GetSingle(x => x.ResourceURL == url);
		}

		protected override dynamic GetCreationSchema()
		{
			return new { RebellionName = "", Location = new GeoLocation() };
		}

		protected override Rebellion CreateFromSchema(HttpRequest request, dynamic info)
		{
			var user = GetRequestOwner(request);
			if(user == null)
			{
				throw HTTPException.LOGIN;
			}
			var newRebellion = new Rebellion(user, info.RebellionName.ToString(), JsonConvert.DeserializeObject<GeoLocation>(info.Location.ToString()));
			DodoServer.RebellionManager.Add(newRebellion);
			return newRebellion;
		}

		protected override void DeleteObjectInternal(Rebellion target)
		{
			DodoServer.RebellionManager.Delete(target);
		}

		protected override bool IsAuthorised(User user, HttpRequest request, Rebellion target)
		{
			// TODO
			return true;
		}
	}
}
