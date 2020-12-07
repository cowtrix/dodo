using Resources;

namespace Dodo
{
	public static class PublishResourceExtensions
	{
		public static IPublicResource SetPublished(this IPublicResource rsc, bool value)
		{
			using var rscLock = new ResourceLock(rsc);
			rsc = rscLock.Value as IPublicResource;
			rsc.IsPublished = value;
			ResourceUtility.GetManager(rsc.GetType()).Update(rsc, rscLock);
			if(rsc is IOwnedResource owned)
			{
				JsonViewUtility.FlushCache(owned.Parent);
			}
			return rsc;
		}
	}
}
