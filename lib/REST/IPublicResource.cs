namespace Resources
{
	public interface IPublicResource : IRESTResource
	{
		bool IsPublished { get; set; }
	}

}
