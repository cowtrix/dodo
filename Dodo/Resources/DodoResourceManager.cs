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
		protected override bool IsAuthorised(HttpRequest request, T resource, out EPermissionLevel permissionLevel)
		{
			var requestOwner = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			return resource.IsAuthorised(requestOwner, passphrase, request, out permissionLevel);
		}
	}
}
