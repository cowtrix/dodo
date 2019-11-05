using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.Users
{
	public class UserManager : ResourceManager<User>
	{
		public override string BackupPath => "users";

		public User CreateNew(WebPortalAuth webAuth)
		{
			webAuth.Validate();
			if (GetSingle(x => x.WebAuth.Username == webAuth.Username) != null)
			{
				throw new Exception("User already exists with username " + webAuth.Username);
			}
			var newUser = new User(webAuth);
			InternalData.Entries[newUser.UUID] = newUser;
			return newUser;
		}

		protected override bool IsAuthorised(HttpRequest request, User resource, out EViewVisibility visibility)
		{
			var requestOwner = DodoRESTServer.GetRequestOwner(request);
			if(requestOwner == resource)
			{
				visibility = EViewVisibility.OWNER;
				return true;
			}
			visibility = EViewVisibility.PUBLIC;
			return request.Method == EHTTPRequestType.GET;
		}
	}
}
