using Dodo.Users;
using Microsoft.AspNetCore.Mvc;
using Resources;

namespace Dodo
{
	public class AuthorizationManager<T, TSchema> 
		where T: IDodoResource 
		where TSchema: DodoResourceSchemaBase
	{
		public virtual ResourceRequest IsAuthorised(AccessContext context, T target, EHTTPRequestType requestType)
		{
			if (target != null && !(target is T))
			{
				return ResourceRequest.BadRequest;
			}
			switch(requestType)
			{
				case EHTTPRequestType.GET:
					return CanGet(context, target);
				case EHTTPRequestType.PATCH:
					return CanEdit(context, target);
				case EHTTPRequestType.DELETE:
					return CanDelete(context, target);
				default:
					throw new System.Exception("Unexpected auth switch fallthrough");  // Incorrect method call, this should never happen
			}
		}

		public virtual ResourceRequest IsAuthorised(AccessContext context, TSchema schema, EHTTPRequestType requestType)
		{
			if (requestType != EHTTPRequestType.POST)
			{
				return ResourceRequest.BadRequest;
			}
			return CanCreate(context, schema);
		}

		protected virtual ResourceRequest CanGet(AccessContext context, T target)
		{
			return new ResourceRequest(context, target, EHTTPRequestType.GET, EPermissionLevel.PUBLIC);
		}

		protected virtual ResourceRequest CanDelete(AccessContext context, T target)
		{
			var permission = GetPermission(context, target);
			if(permission >= EPermissionLevel.OWNER)
			{
				return new ResourceRequest(context, target, EHTTPRequestType.DELETE, permission);
			}
			return ResourceRequest.ForbidRequest;
		}

		protected virtual ResourceRequest CanEdit(AccessContext context, T target)
		{
			var permission = GetPermission(context, target);
			if (permission >= EPermissionLevel.ADMIN)
			{
				return new ResourceRequest(context, target, EHTTPRequestType.PATCH, permission);
			}
			return ResourceRequest.ForbidRequest;
		}

		protected virtual ResourceRequest CanCreate(AccessContext context, TSchema target)
		{
			return ResourceRequest.ForbidRequest;
		}

		protected virtual EPermissionLevel GetPermission(AccessContext context, T target)
		{
			return EPermissionLevel.PUBLIC;
		}
	}
}
