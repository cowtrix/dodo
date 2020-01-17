using Dodo.Utility;
using Microsoft.AspNetCore.Http;
using REST;

namespace Dodo.Resources
{
	public abstract class DodoResourceManager<T> : ResourceManager<T> where T : class, IDodoResource
	{
		protected override bool IsAuthorised(HttpRequest request, T resource, out EPermissionLevel visibility)
		{
			var context = request.GetRequestOwner();
			if(context.User != null && !context.User.EmailVerified && request.MethodEnum() != EHTTPRequestType.GET)
			{
				visibility = EPermissionLevel.PUBLIC;
				return false;
			}
			return resource.IsAuthorised(context, request, out visibility);
		}

		protected override string MongoDBDatabaseName => Dodo.PRODUCT_NAME;
	}
}
