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

		public class CreationSchema : IRESTResourceSchema
		{
			public string Username = "";
			public string Password = "";
			public string Name = "";
			public string Email = "";
		}

		[Route("Register a new user", "^" + CREATION_URL, EHTTPRequestType.POST)]
		public HttpResponse Register(HttpRequest request)
		{
			return CreateObject(request);
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema();
		}

		protected override User CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var password = info.Password.ToString();
			if (!ValidationExtensions.StrongPassword(password, out string error))
			{
				throw new Exception(error);
			}
			var email = info.Email.ToString();
			if (!ValidationExtensions.EmailIsValid(info.Email.ToString()))
			{
				throw new Exception("Invalid email address");
			}
			var username = info.Username.ToString();
			if (!ValidationExtensions.UsernameIsValid(username, out error))
			{
				throw new Exception(error);
			}
			if(!ValidationExtensions.NameIsValid(info.Name, out error))
			{
				throw new Exception(error);
			}
			var newUser = new User(new WebPortalAuth(username, password), password);
			newUser.Email = email;
			newUser.Name = info.Name.ToString();
			DodoServer.ResourceManager<User>().Add(newUser);
			return newUser;
		}
	}
}
