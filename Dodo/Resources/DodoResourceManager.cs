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
	public abstract class DodoResourceManager<T> : ResourceManager<T> where T : DodoResource
	{
		protected override bool IsAuthorised(HttpRequest request, T resource, out EPermissionLevel visibility)
		{
			var requestOwner = DodoRESTServer.GetRequestOwner(request);
			return resource.IsAuthorised(requestOwner, request, out visibility);
		}
	}
}
