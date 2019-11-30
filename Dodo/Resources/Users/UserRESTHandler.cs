using Common;
using Common.Extensions;
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
		public const string CREATION_URL = "^register$";

		public class CreationSchema : IRESTResourceSchema
		{
			public string Username = "";
			public string Password = "";
			public string Name = "";
			public string Email = "";
		}

		public HttpResponse ResetPassword(HttpRequest request)
		{
			throw new NotImplementedException();
			if(DodoRESTServer.GetRequestOwner(request) != null)
			{
				return HttpBuilder.Forbidden();
			}
			var email = JsonExtensions.DeserializeAnonymousType(request.Content, new { Email = "" }).Email;
			var userWithEmail = ResourceUtility.GetManager<User>().GetSingle(x => x.Email == email);
			if(userWithEmail == null)
			{
				return HttpBuilder.NotFound();
			}
			userWithEmail.WebAuth.PasswordResetToken = WebPortalAuth.ResetToken.Generate();
			// TODO send an email with the auth link
			return HttpBuilder.OK();
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema();
		}

		protected override User CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var newUser = new User(info);
			if (URLIsCreation(newUser.ResourceURL))
			{
				throw new Exception("Reserved Resource URL");
			}
			newUser.Verify();
			return newUser;
		}

		protected override bool URLIsCreation(string url)
		{
			return url == "register";
		}
	}
}
