namespace Resources
{
	public static class ResourceExtensions
	{
		public static ResourceReference<T> CreateRef<T>(this T rsc) where T:class, IRESTResource
		{
			return new ResourceReference<T>(rsc);
		}

		public static bool IsHidden(this IPublicResource pub)
		{
			return ResourceUtility.Hidden.ContainsKey(pub.Guid);
		}

		public static void Hide(this IPublicResource pub, string reason)
		{
			ResourceUtility.Hidden[pub.Guid] = reason;
		}

		public static void UnHide(this IPublicResource pub)
		{
			ResourceUtility.Hidden.Remove(pub.Guid);
		}
	}
}
