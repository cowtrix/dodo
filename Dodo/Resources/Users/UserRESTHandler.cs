using Common;
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
		public const string CREATION_URL = "register";
		const string URL_REGEX = User.ROOT + "/(?:^/)*";

		[Route("Register a new user", "^" + CREATION_URL, EHTTPRequestType.POST)]
		public HttpResponse Register(HttpRequest request)
		{
			return CreateObject(request);
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
			return new { Username = "", Name = "", Password = "", Email = "" };
		}

		protected override User CreateFromSchema(HttpRequest request, dynamic info)
		{
			if (!ValidationExtensions.StrongPassword(info.Password.ToString(), out string error))
			{
				throw new Exception(error);
			}
			if (!ValidationExtensions.EmailIsValid(info.Email.ToString()))
			{
				throw new Exception("Invalid email address");
			}
			var newUser = new User(new WebPortalAuth(info.Username.ToString(), info.Password.ToString()), info.Password.ToString());
			newUser.Email = info.Email;
			newUser.Name = info.Name.ToString();
			DodoServer.ResourceManager<User>().Add(newUser);
			return newUser;
		}

		protected override void DeleteObjectInternal(User target)
		{
			DodoServer.ResourceManager<User>().Delete(target);
		}
	}
}
