using Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Dodo.Users
{
	public class UserAuthManager : AuthorizationManager<User, UserSchema>
	{
		public UserAuthManager(ControllerContext controllercontext, HttpRequest request) :
			base(controllercontext, request)
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

		public override ResourceRequest IsAuthorised(AccessContext context, User target, EHTTPRequestType requestType, string action)
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
