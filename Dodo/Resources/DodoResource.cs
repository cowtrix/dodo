using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Collections.Concurrent;

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
		public virtual ConcurrentBag<ResourceReference<User>> Administrators { get; }
		public abstract bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel visibility);
	}

	/// <summary>
	/// A group resource is either a Working Group or a Local Group
	/// This common class pretty much exists to support Roles, as they
	/// can have a reference to either
	/// </summary>
	public abstract class GroupResource : DodoResource
	{
		public GroupResource(User creator) : base(creator)
		{
		}
	}
}
