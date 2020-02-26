using Resources;
using Microsoft.AspNetCore.Mvc;

namespace Dodo.Users
{
	public class UserAuthManager : AuthorizationManager<User>
	{
		protected override ResourceRequest CanGet(AccessContext context, User target, EHTTPRequestType requestType)
		{
			if(target.GUID == context.User.GUID)
			{
				return new ResourceRequest(context, target, requestType, EPermissionLevel.OWNER);
			}
			return new ResourceRequest(new ForbidResult());
		}

		protected override ResourceRequest CanDelete(AccessContext context, User target, EHTTPRequestType requestType)
		{
			if (target.GUID == context.User.GUID)
			{
				return new ResourceRequest(context, target, requestType, EPermissionLevel.OWNER);
			}
			return new ResourceRequest(new ForbidResult());
		}

		protected override ResourceRequest CanEdit(AccessContext context, User target, EHTTPRequestType requestType)
		{
			if (target.GUID == context.User.GUID)
			{
				return new ResourceRequest(context, target, requestType, EPermissionLevel.OWNER);
			}
			return new ResourceRequest(new ForbidResult());
		}

		protected override ResourceRequest IsAuthorisedAnon(AccessContext context, User target, EHTTPRequestType requestType)
		{
			if (requestType == EHTTPRequestType.POST)
			{
				// User register
				return new ResourceRequest(context, target, requestType, EPermissionLevel.OWNER);
			}
			// Deny everything else
			return new ResourceRequest(new ForbidResult());
		}
	}
}
