using Microsoft.AspNetCore.Http;
using Resources;

namespace Dodo.Resources
{
	public abstract class DodoResourceManager<T> : ResourceManager<T> where T : class, IDodoResource
	{
		protected override string MongoDBDatabaseName => Dodo.PRODUCT_NAME;
	}
}
