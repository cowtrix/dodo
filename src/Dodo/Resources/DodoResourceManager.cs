using Dodo.Utility;
using Microsoft.AspNetCore.Http;
using REST;

namespace Dodo.Resources
{
	public abstract class DodoResourceManager<T> : ResourceManager<T> where T : class, IDodoResource
	{
		protected override bool IsAuthorised(HttpRequest request, T resource, out EPermissionLevel visibility)
		{
			var requestOwner = request.GetRequestOwner(out var passphrase);
			if(requestOwner != null && !requestOwner.EmailVerified && request.MethodEnum() != EHTTPRequestType.GET)
			{
				visibility = EPermissionLevel.PUBLIC;
				return false;
			}
			return resource.IsAuthorised(requestOwner, passphrase, request, out visibility);
		}

		protected override string MongoDBDatabaseName => Dodo.PRODUCT_NAME;
	}
}
