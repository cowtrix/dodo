using Resources;

namespace Dodo
{
	public interface IPublicResource : IDodoResource
	{
		bool IsPublished { get; set; }
	}

	public static class PublishResourceExtensions
	{
		public static void Publish(this IPublicResource rsc)
		{
			using var rscLock = new ResourceLock(rsc);
			rsc = rscLock.Value as IPublicResource;
			rsc.IsPublished = true;
			ResourceUtility.GetManager(rsc.GetType()).Update(rsc, rscLock);
		}
	}
}
