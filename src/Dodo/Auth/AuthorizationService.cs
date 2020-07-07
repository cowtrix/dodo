using Resources;
using System;

namespace Dodo
{
	public class AuthorizationService<T, TSchema>
		where T : class, IDodoResource
		where TSchema : ResourceSchemaBase
	{
		protected virtual IResourceManager<T> ResourceManager => ResourceUtility.GetManager<T>();
		public AuthorizationService()
		{
		}

		public virtual IRequestResult IsAuthorised(AccessContext context, string targetID, EHTTPRequestType requestType, string action = null)
		{
			T rsc = null;
			if(Guid.TryParse(targetID, out var guid))
			{
				rsc = ResourceManager.GetSingle(r => r.Guid == guid);
			}
			else
			{
				rsc = ResourceManager.GetSingle(r => r.Slug == targetID);
			}
			return IsAuthorised(context, rsc, requestType, action);
		}

		public virtual IRequestResult IsAuthorised(AccessContext context, T target, EHTTPRequestType requestType, string action = null)
		{
			if (target != null && !(target is T))
			{
				return ResourceRequestError.BadRequest();
			}
			switch (requestType)
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

		public virtual IRequestResult IsAuthorised(AccessContext context, TSchema schema, EHTTPRequestType requestType)
		{
			if (requestType != EHTTPRequestType.POST)
			{
				return ResourceRequestError.BadRequest();
			}
			return CanCreate(context, schema);
		}

		protected virtual IRequestResult CanPost(AccessContext context, T target, string action = null)
		{
			if (context.User == null)
			{
				return ResourceRequestError.ForbidRequest();
			}
			return ResourceRequestError.UnauthorizedRequest();
		}

		protected virtual IRequestResult CanGet(AccessContext context, T target, string action = null)
		{
			return new ResourceActionRequest(context, target, EHTTPRequestType.GET, GetPermission(context, target), action);
		}

		protected virtual IRequestResult CanDelete(AccessContext context, T target)
		{
			var permission = GetPermission(context, target);
			if (permission >= EPermissionLevel.OWNER)
			{
				return new ResourceActionRequest(context, target, EHTTPRequestType.DELETE, permission);
			}
			if (context.User == null)
			{
				return ResourceRequestError.ForbidRequest();
			}
			return ResourceRequestError.UnauthorizedRequest();
		}

		protected virtual IRequestResult CanEdit(AccessContext context, T target)
		{
			var permission = GetPermission(context, target);
			if (permission >= EPermissionLevel.ADMIN)
			{
				return new ResourceActionRequest(context, target, EHTTPRequestType.PATCH, permission);
			}
			if (context.User == null)
			{
				return ResourceRequestError.ForbidRequest();
			}
			return ResourceRequestError.UnauthorizedRequest();
		}

		protected virtual IRequestResult CanCreate(AccessContext context, TSchema target)
		{
			if (context.User == null)
			{
				return ResourceRequestError.ForbidRequest();
			}
			return ResourceRequestError.UnauthorizedRequest();
		}

		protected virtual EPermissionLevel GetPermission(AccessContext context, T target)
		{
			if (target != null && target.IsCreator(context))
			{
				return EPermissionLevel.OWNER;
			}
			if (context.User != null)
			{
				return EPermissionLevel.USER;
			}
			return EPermissionLevel.PUBLIC;
		}
	}
}
