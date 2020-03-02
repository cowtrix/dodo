using Resources;
using Dodo.Roles;
using Dodo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DodoResources.Roles
{
	public class RoleAuthManager : AuthorizationManager<Role, RoleSchema>
	{
		public RoleAuthManager(ControllerContext controllercontext, HttpRequest request) : 
			base(controllercontext, request)
		{
		}

		protected override ResourceRequest CanCreate(AccessContext context, RoleSchema target)
		{
			var parent = ResourceUtility.GetResourceByGuid(target.Parent) as GroupResource;
			if (parent == null)
			{
				return ResourceRequest.BadRequest;
			}
			if (!parent.IsAdmin(context.User, context))
			{
				return ResourceRequest.ForbidRequest;
			}
			return new ResourceRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.OWNER);
		}
	}
}
