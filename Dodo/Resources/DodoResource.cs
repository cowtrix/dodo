using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo
{
	public abstract class DodoResource : Resource
	{
		public abstract bool IsAuthorised(User requestOwner, HttpRequest request, out EViewVisibility visibility);
	}
}
