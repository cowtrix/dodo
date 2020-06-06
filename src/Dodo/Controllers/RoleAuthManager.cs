using Resources;
using Dodo.Roles;
using Dodo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DodoResources.Roles
{
	public class RoleAuthManager : AuthorizationService<Role, RoleSchema>
	{
		public RoleAuthManager() : base()
		{
		}

		protected override IRequestResult CanCreate(AccessContext context, RoleSchema target)
		{
			var parent = ResourceUtility.GetResourceByGuid(target.Parent) as GroupResource;
			if (parent == null)
			{
				return ResourceRequestError.BadRequest();
			}
			if (!parent.IsAdmin(context.User, context))
			{
				return ResourceRequest.ForbidRequest;
			}
			return new ResourceRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.OWNER);
		}
	}
}
