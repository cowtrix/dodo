using Resources;

namespace Dodo
{
	public interface IMediaResource : IRESTResource
	{
		string VideoEmbedURL { get; }
		string PhotoEmbedURL { get; }
	}
}
