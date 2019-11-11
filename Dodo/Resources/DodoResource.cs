using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo
{
	public abstract class DodoResource : Resource
	{
		public DodoResource(User creator)
		{
			Creator = new ResourceReference<User>(creator);
		}
		public ResourceReference<User> Creator { get; private set; }
		public abstract bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel visibility);
	}
}
