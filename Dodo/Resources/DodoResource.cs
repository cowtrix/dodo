using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Collections.Concurrent;

namespace Dodo
{
	public interface IDodoResource : IRESTResource
	{
		ResourceReference<User> Creator { get; }
		bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel permissionLevel);
	}

	public abstract class DodoResource : Resource, IDodoResource
	{
		public DodoResource(User creator)
		{
			Creator = new ResourceReference<User>(creator);
		}
		public ResourceReference<User> Creator { get; private set; }
		public abstract bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel permissionLevel);
	}
}
