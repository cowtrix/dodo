using Dodo.Users.Tokens;
using Resources;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Dodo.RoleApplications
{
	public class RoleApplicationAuthService : AuthorizationService<RoleApplication, RoleApplicationSchema>
	{
		public RoleApplicationAuthService() : base()
		{
		}

		protected override IRequestResult CanCreate(AccessContext context, RoleApplicationSchema target) => ResourceRequestError.BadRequest($"Applications must be created by user interaction");
		
		protected override EPermissionLevel GetPermission(AccessContext context, RoleApplication target)
		{
			if(target.IsCreator(context))
			{
				return EPermissionLevel.OWNER;
			}
			var parentGroup = ResourceUtility.GetResourceByGuid<IAdministratedResource>(target.Parent.Parent);
			if(parentGroup.IsAdmin(context.User, context, out var permissions) && permissions.CanManageRoles)
			{
				return EPermissionLevel.ADMIN;
			}
			return EPermissionLevel.PUBLIC;
		}

		public override IRequestResult IsAuthorised(AccessContext context, RoleApplication target, EHTTPRequestType requestType, string action)
		{
			if(target == null)
			{
				return ResourceRequestError.NotFoundRequest();
			}
			var permission = GetPermission(context, target);
			if (permission < EPermissionLevel.ADMIN)	 // only admins and applicant
			{
				if (context.User == null)
				{
					return ResourceRequestError.ForbidRequest();
				}
				return ResourceRequestError.UnauthorizedRequest();
			}
			return base.IsAuthorised(context, target, requestType, action);
		}
	}
}
