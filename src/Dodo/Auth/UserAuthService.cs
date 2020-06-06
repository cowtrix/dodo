using Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Dodo.Users
{
	public class UserAuthService : AuthorizationService<User, UserSchema>
	{
		public UserAuthService() : base()
		{
		}

		protected override EPermissionLevel GetPermission(AccessContext context, User target)
		{
			if(context.User != null && context.User.Guid == target.Guid)
			{
				return EPermissionLevel.OWNER;
			}
			return EPermissionLevel.PUBLIC;
		}

		public override IRequestResult IsAuthorised(AccessContext context, User target, EHTTPRequestType requestType, string action)
		{
			var permission = GetPermission(context, target);
			if(permission != EPermissionLevel.OWNER)
			{
				if (context.User == null)
				{
					return ResourceRequestError.ForbidRequest();
				}
				return ResourceRequestError.UnauthorizedRequest();
			}
			return new ResourceActionRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.OWNER);
		}

		protected override IRequestResult CanCreate(AccessContext context, UserSchema target)
		{
			if(context.User == null)
			{
				return new ResourceCreationRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.OWNER);
			}
			return ResourceRequestError.BadRequest();
		}
	}
}
