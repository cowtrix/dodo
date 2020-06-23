using Resources;
using Dodo.Roles;
using Dodo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DodoResources.Roles
{
	public class RoleAuthService : AuthorizationService<Role, RoleSchema>
	{
		public RoleAuthService() : base()
		{
		}

		protected override IRequestResult CanCreate(AccessContext context, RoleSchema target)
		{
			var parent = ResourceUtility.GetResourceByGuid(target.Parent) as GroupResource;
			if (parent == null)
			{
				return ResourceRequestError.BadRequest();
			}
			if (!parent.IsAdmin(context.User, context, out var p) || !p.CanCreateChildObjects)
			{
				return ResourceRequestError.UnauthorizedRequest();
			}
			return new ResourceCreationRequest(context, target);
		}
	}
}
