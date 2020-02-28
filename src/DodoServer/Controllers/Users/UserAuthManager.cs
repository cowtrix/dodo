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
