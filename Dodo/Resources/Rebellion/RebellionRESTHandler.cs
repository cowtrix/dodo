using Dodo.Users;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Text.RegularExpressions;

namespace Dodo.Rebellions
{
	public class RebellionRESTHandler : DodoRESTHandler<Rebellion>
	{
		const string URL_REGEX = "rebellions/(?:^/)*";

		[Route("Create a new rebellion", "rebellions/create", EHTTPRequestType.POST)]
		public HttpResponse Register(HttpRequest request)
		{
			return CreateObject(request);
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
			return new {  };
		}

		protected override Rebellion CreateFromSchema(dynamic info)
		{
		}

		protected override void DeleteObjectInternal(Rebellion target)
		{

		}

		protected override bool IsAuthorised(User user, EHTTPRequestType requestType, Rebellion target)
		{
			// TODO
			return true;
		}
	}
}
