using Resources;
using Microsoft.AspNetCore.Mvc;

namespace Dodo.Users
{
	public class UserAuthManager : AuthorizationManager<User, UserSchema>
	{
		protected override EPermissionLevel GetPermission(AccessContext context, User target)
		{
			if(context.User.GUID == target.GUID)
			{
				return EPermissionLevel.OWNER;
			}
			return EPermissionLevel.PUBLIC;
		}

		public override ResourceRequest IsAuthorised(AccessContext context, User target, EHTTPRequestType requestType)
		{
			var permission = GetPermission(context, target);
			if(permission != EPermissionLevel.OWNER)
			{
				if (context.User == null)
				{
					return ResourceRequest.ForbidRequest;
				}
				return ResourceRequest.UnauthorizedRequest;
			}
			return new ResourceRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.OWNER);
		}

		protected override ResourceRequest CanCreate(AccessContext context, UserSchema target)
		{
			if(context.User == null)
			{
				return new ResourceRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.OWNER);
			}
			return ResourceRequest.BadRequest;
		}
	}
}
