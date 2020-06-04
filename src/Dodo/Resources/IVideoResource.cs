using Resources;

namespace Dodo
{
	public interface IVideoResource : IRESTResource
	{
		string VideoEmbedURL { get; }
	}
}
