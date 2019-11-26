using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodo.Resources
{
	public abstract class DodoResourceManager<T> : ResourceManager<T> where T : class, IDodoResource
	{
		protected override bool IsAuthorised(HttpRequest request, T resource, out EUserPriviligeLevel permissionLevel)
		{
			var requestOwner = DodoRESTServer.GetRequestOwner(request);
			return resource.IsAuthorised(requestOwner, request, out permissionLevel);
		}
	}
}
