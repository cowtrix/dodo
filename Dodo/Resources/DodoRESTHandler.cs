using System.Collections.Generic;
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
		protected override bool UrlIsMatch(string url)
		{
			return GetResource(url) != null;
		}

		protected override T GetResource(string url)
		{
			var rm = DodoServer.ResourceManager<T>();
			return rm.GetSingle(x => x.ResourceURL == url);
		}

		protected override void DeleteObjectInternal(T target)
		{
			var rm = DodoServer.ResourceManager<T>();
			rm.Delete(target);
		}

		protected override bool IsAuthorised(HttpRequest request, out EPermissionLevel visibility, out object context, out string passphrase)
		{
			var target = GetResource(request.Url);
			context = null;
			passphrase = null;
			if(target == null)
			{
				// TODO
				if (request.Method == EHTTPRequestType.POST)
				{
					visibility = EPermissionLevel.OWNER;
				}
				else
				{
					visibility = EPermissionLevel.PUBLIC;
				}
				return true;
			}
			context = DodoRESTServer.GetRequestOwner(request, out passphrase);
			var rm = ResourceUtility.GetManagerForResource(target);
			return rm.IsAuthorised(request, target, out visibility);
		}
	}
}
