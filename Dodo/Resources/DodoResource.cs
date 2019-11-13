using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo
{
	public interface IDodoResource : IRESTResource
	{
		ResourceReference<User> Creator { get; }
		bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel visibility);
	}

	public abstract class DodoResource : Resource, IDodoResource
	{
		public DodoResource(User creator)
		{
			Creator = new ResourceReference<User>(creator);
		}
		public ResourceReference<User> Creator { get; private set; }
		public abstract bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel visibility);
	}
}
