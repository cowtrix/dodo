using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo.Rebellions
{
	public abstract class RebellionResource : GroupResource
	{
		public Rebellion Rebellion { get; private set; }
		public RebellionResource(User creator, Rebellion owner) : base(creator)
		{
			Rebellion = owner;
		}

		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel visibility)
		{
			return Rebellion.IsAuthorised(requestOwner, request, out visibility);
		}
	}
}
