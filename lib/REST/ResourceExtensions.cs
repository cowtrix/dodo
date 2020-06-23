namespace Resources
{
	public static class ResourceExtensions
	{
		public static ResourceReference<T> CreateRef<T>(this T rsc) where T:class, IRESTResource
		{
			return new ResourceReference<T>(rsc);
		}
	}
}
