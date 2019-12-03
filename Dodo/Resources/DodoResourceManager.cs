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
		protected override bool IsAuthorised(HttpRequest request, T resource, out EPermissionLevel visibility)
		{
			var requestOwner = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			if(requestOwner != null && !requestOwner.EmailVerified && request.Method != SimpleHttpServer.EHTTPRequestType.GET)
			{
				visibility = EPermissionLevel.PUBLIC;
				return false;
			}
			return resource.IsAuthorised(requestOwner, passphrase, request, out visibility);
		}
	}
}
