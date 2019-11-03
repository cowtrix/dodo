using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Text.RegularExpressions;

namespace Dodo.Users
{
	public class UserRESTHandler : DodoRESTHandler<User>
	{
		const string URL_REGEX = "u/(?:^/)*";

		[Route("Register a new user", "register", EHTTPRequestType.POST)]
		public HttpResponse Register(HttpRequest request)
		{
			return CreateObject(request);
		}

		[Route("Get a user", URL_REGEX, EHTTPRequestType.GET)]
		public HttpResponse GetUser(HttpRequest request)
		{
			var user = GetResource(request.Url);
			if(user == null)
			{
				throw HTTPException.NOT_FOUND;
			}
			return HttpBuilder.OK(user.GenerateJsonView());
		}

		[Route("Delete a user", URL_REGEX, EHTTPRequestType.DELETE)]
		public HttpResponse DeleteUser(HttpRequest request)
		{
			return DeleteObject(request);
		}

		[Route("Update a user", URL_REGEX, EHTTPRequestType.PATCH)]
		public HttpResponse UpdateUser(HttpRequest request)
		{
			return UpdateObject(request);
		}

		protected override User GetResource(string url)
		{
			if(!Regex.IsMatch(url, URL_REGEX))
			{
				return null;
			}
			var username = url.Substring(url.LastIndexOf('/') + 1);
			var sessionManager = DodoServer.SessionManager;
			return sessionManager.GetSingle(x => x.WebAuth.Username == username);
		}

		protected override dynamic GetCreationSchema()
		{
			return new { Username = "", Password = "" };
		}

		protected override User CreateFromSchema(HttpRequest request, dynamic info)
		{
			return DodoServer.SessionManager.CreateNew(new WebPortalAuth(info.Username.ToString(), info.Password.ToString()));
		}

		protected override void DeleteObjectInternal(User target)
		{
			DodoServer.SessionManager.Delete(target);
		}

		protected override bool IsAuthorised(User user, EHTTPRequestType requestType, User target)
		{
			// TODO
			return true;
		}
	}
}
