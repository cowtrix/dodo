using Resources;

namespace Dodo.DodoResources
{
	public abstract class DodoResourceManager<T> : ResourceManager<T> where T : class, IDodoResource
	{
		protected override string MongoDBDatabaseName => DodoApp.PRODUCT_NAME;
	}
}
