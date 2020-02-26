using Dodo.Users;
using Microsoft.AspNetCore.Mvc;
using Resources;

namespace Dodo
{
	public class AuthorizationManager<T> where T: IDodoResource
	{
		public virtual ResourceRequest IsAuthorised(AccessContext context, T target, EHTTPRequestType requestType)
		{
			if (target != null && !(target is T))
			{
				return new ResourceRequest(new BadRequestResult());
			}
			
			if(context.User == null)
			{
				return IsAuthorisedAnon(context, target, requestType);
			}
			
			switch(requestType)
			{
				case EHTTPRequestType.GET:
					return CanGet(context, target, requestType);
				case EHTTPRequestType.POST:
					return CanCreate(context, target, requestType);
				case EHTTPRequestType.PATCH:
					return CanEdit(context, target, requestType);
				case EHTTPRequestType.DELETE:
					return CanDelete(context, target, requestType);
				default:
					throw new System.Exception("Unexpected switch result");
			}
		}

		protected virtual ResourceRequest CanGet(AccessContext context, T target, EHTTPRequestType requestType)
		{
			return new ResourceRequest(context, target, requestType, EPermissionLevel.PUBLIC);
		}

		protected virtual ResourceRequest CanDelete(AccessContext context, T target, EHTTPRequestType requestType)
		{
			return new ResourceRequest(new UnauthorizedResult());
		}

		protected virtual ResourceRequest CanEdit(AccessContext context, T target, EHTTPRequestType requestType)
		{
			return new ResourceRequest(new UnauthorizedResult());
		}

		protected virtual ResourceRequest CanCreate(AccessContext context, T target, EHTTPRequestType requestType)
		{
			return new ResourceRequest(new UnauthorizedResult());
		}

		protected virtual ResourceRequest IsAuthorisedAnon(AccessContext context, T target, EHTTPRequestType requestType)
		{
			if(requestType == EHTTPRequestType.GET)
			{
				// If it's just GET then return a public view
				return new ResourceRequest(context, target, requestType, EPermissionLevel.PUBLIC);
			}
			// Deny everything else
			return new ResourceRequest(new ForbidResult());
		}
	}
}
