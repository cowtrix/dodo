using Dodo.Resources;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Linq;
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
			var owner = DodoRESTServer.GetRequestOwner(request);
			var user = GetResource(request.Url);
			if(user == null)
			{
				throw HTTPException.NOT_FOUND;
			}
			if (!user.IsAuthorised(owner, request, out var view))
			{
				throw HTTPException.FORBIDDEN;
			}
			return HttpBuilder.OK(user.GenerateJsonView(view));
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
			if(url.StartsWith("/"))
			{
				url = url.Substring(1);
			}
			var username = url.ToLowerInvariant();
			var sessionManager = DodoServer.ResourceManager<User>();
			return sessionManager.GetSingle(x => x.ResourceURL == username);
		}

		protected override dynamic GetCreationSchema()
		{
			return new { Username = "", Password = "", Email = "" };
		}

		protected override User CreateFromSchema(HttpRequest request, dynamic info)
		{
			var newUser = new User(new WebPortalAuth(info.Username.ToString(), info.Password.ToString()));
			newUser.Email = info.Email;
			DodoServer.ResourceManager<User>().Add(newUser);
			return newUser;
		}

		protected override void DeleteObjectInternal(User target)
		{
			DodoServer.ResourceManager<User>().Delete(target);
		}
	}
}
