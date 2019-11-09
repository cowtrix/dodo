using Common;
using Dodo.Resources;
using Dodo.Users;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo
{
	public abstract class DodoRESTHandler<T> : ObjectRESTHandler<T> where T: DodoResource, IRESTResource
	{
		protected override bool IsAuthorised(HttpRequest request, out EViewVisibility visibility, out object context, out string passphrase)
		{
			var target = GetResource(request.Url);
			context = null;
			passphrase = null;
			if(target == null)
			{
				// TODO
				if (request.Method == EHTTPRequestType.POST)
				{
					visibility = EViewVisibility.OWNER;
				}
				else
				{
					visibility = EViewVisibility.HIDDEN;
				}
				return true;
			}
			context = DodoRESTServer.GetRequestOwner(request, out passphrase);
			var rm = ResourceUtility.GetManagerForResource(target);
			return rm.IsAuthorised(request, target, out visibility);
		}
	}
}
