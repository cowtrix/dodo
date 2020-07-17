using Resources;

namespace Dodo
{
	public interface IDodoResource : IRESTResource
	{
		bool IsCreator(AccessContext context);
	}
}
