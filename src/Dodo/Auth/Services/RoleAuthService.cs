using Dodo.Roles;
using Resources;

namespace Dodo
{
	public class RoleAuthService : OwnedResourceAuthService<Role, RoleSchema>
	{
		public RoleAuthService() : base()
		{
		}

		protected override IRequestResult CanPost(AccessContext context, Role target, string action = null)
		{
			if(target == null)
			{
				return ResourceRequestError.NotFoundRequest();
			}
			if(context.User == null)
			{
				return ResourceRequestError.ForbidRequest();
			}
			if(action == RoleService.APPLY)
			{
				return new ResourceActionRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.USER, action);
			}
			return ResourceRequestError.NotFoundRequest();
		}
	}
}
