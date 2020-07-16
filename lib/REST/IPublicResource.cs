namespace Resources
{
	public interface IPublicResource : IDescribedResource
	{
		public const string PublishInputHint = "This indicates whether or not this is publicly viewable by all users.";
		bool IsPublished { get; set; }
	}
}
