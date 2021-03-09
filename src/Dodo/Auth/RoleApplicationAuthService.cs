using Dodo.Users.Tokens;
using Resources;
using Dodo.RoleApplications;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Dodo.RoleApplications
{
	public class RoleApplicationAuthService : AuthorizationService<RoleApplication, RoleApplicationSchema>
	{
		public RoleApplicationAuthService() : base()
		{
		}

		protected override IRequestResult CanCreate(AccessContext context, RoleApplicationSchema target)
		{
			if(target.Role == null)
			{
				return ResourceRequestError.NotFoundRequest();
			}
			if(target.Role.HasApplied(context, out _, out _))
			{
				return ResourceRequestError.Conflict();
			}
			return new ResourceCreationRequest(context, target);
		}
		
		protected override EPermissionLevel GetPermission(AccessContext context, RoleApplication target)
		{
			if(target.IsCreator(context))
			{
				return EPermissionLevel.ADMIN;
			}
			var parentGroup = ResourceUtility.GetResourceByGuid<IAdministratedResource>(target.Parent.Parent);
			if(parentGroup.IsAdmin(context.User, context, out _))
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

		protected override IRequestResult CanPost(AccessContext context, RoleApplication target, string action = null)
		{
			if(action == RoleApplication.MESSAGE)
			{
				var permission = GetPermission(context, target);
				if(permission < EPermissionLevel.ADMIN)
				{
					return context.User == null ? ResourceRequestError.ForbidRequest() : ResourceRequestError.UnauthorizedRequest();
				}
				return new ResourceActionRequest(context, target, EHTTPRequestType.POST, permission, action);
			}
			if (action == RoleApplicationService.APPLY)
			{
				return new ResourceActionRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.USER, action);
			}
			return base.CanPost(context, target, action);
		}
	}
}
