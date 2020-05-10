using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resources;
using System;

namespace Dodo
{
	public static class AuthorizationManager
	{
		public static AuthorizationService<T, TSchema> GetAuthService<T, TSchema>()
			where T : IDodoResource
			where TSchema : ResourceSchemaBase
		{
			throw new NotImplementedException();
		}
	}

	public class AuthorizationService<T, TSchema> 
		where T: IDodoResource 
		where TSchema: ResourceSchemaBase
	{
		public AuthorizationService()
		{
		}

		public virtual ResourceRequest IsAuthorised(AccessContext context, T target, EHTTPRequestType requestType, string action = null)
		{
			if (target != null && !(target is T))
			{
				return ResourceRequest.BadRequest;
			}
			switch(requestType)
			{
				case EHTTPRequestType.GET:
					return CanGet(context, target, action);
				case EHTTPRequestType.PATCH:
					return CanEdit(context, target);
				case EHTTPRequestType.DELETE:
					return CanDelete(context, target);
				case EHTTPRequestType.POST:
					return CanPost(context, target, action);
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

		protected virtual ResourceRequest CanPost(AccessContext context, T target, string action = null)
		{
			if(context.User == null)
			{
				return ResourceRequest.ForbidRequest;
			}
			return ResourceRequest.UnauthorizedRequest;
		}

		protected virtual ResourceRequest CanGet(AccessContext context, T target, string action = null)
		{
			return new ResourceRequest(context, target, EHTTPRequestType.GET, GetPermission(context, target));
		}

		protected virtual ResourceRequest CanDelete(AccessContext context, T target)
		{
			var permission = GetPermission(context, target);
			if(permission >= EPermissionLevel.OWNER)
			{
				return new ResourceRequest(context, target, EHTTPRequestType.DELETE, permission);
			}
			if(context.User == null)
			{
				return ResourceRequest.ForbidRequest;
			}
			return ResourceRequest.UnauthorizedRequest;
		}

		protected virtual ResourceRequest CanEdit(AccessContext context, T target)
		{
			var permission = GetPermission(context, target);
			if (permission >= EPermissionLevel.ADMIN)
			{
				return new ResourceRequest(context, target, EHTTPRequestType.PATCH, permission);
			}
			if (context.User == null)
			{
				return ResourceRequest.ForbidRequest;
			}
			return ResourceRequest.UnauthorizedRequest;
		}

		protected virtual ResourceRequest CanCreate(AccessContext context, TSchema target)
		{
			if (context.User == null)
			{
				return ResourceRequest.ForbidRequest;
			}
			return ResourceRequest.UnauthorizedRequest;
		}

		protected virtual EPermissionLevel GetPermission(AccessContext context, T target)
		{
			if(target != null && target.IsCreator(context))
			{
				return EPermissionLevel.OWNER;
			}
			if(context.User != null)
			{
				return EPermissionLevel.USER;
			}
			return EPermissionLevel.PUBLIC;
		}
	}
}
