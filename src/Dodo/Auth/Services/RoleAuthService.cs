using Dodo.Roles;
using Dodo.Users.Tokens;
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
			// Everything below requires admin
			if (!target.Parent.GetValue<IAdministratedResource>().IsAdmin(context.User, context, out var permissionSet) || !permissionSet.CanManageRoles)
			{
				return ResourceRequestError.UnauthorizedRequest("You either aren't an administrator, or you don't have the CanManageRoles permission");
			}
			if (action == INotificationResource.ACTION_NOTIFICATION && permissionSet.CanManageAnnouncements)
			{
				return new ResourceActionRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.ADMIN, action);
			}
			return ResourceRequestError.UnauthorizedRequest();
		}
	}
}
