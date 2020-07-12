using Resources;

namespace Dodo
{

	public static class PublishResourceExtensions
	{
		public static IPublicResource Publish(this IPublicResource rsc)
		{
			using var rscLock = new ResourceLock(rsc);
			rsc = rscLock.Value as IPublicResource;
			rsc.IsPublished = true;
			ResourceUtility.GetManager(rsc.GetType()).Update(rsc, rscLock);
			return rsc;
		}
	}
}