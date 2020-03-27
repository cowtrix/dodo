using Resources;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using Dodo.Users;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Dodo.Users.Tokens;

namespace DodoResources
{
	public class GroupResourceAuthManager<T, TSchema> : AuthorizationManager<T, TSchema>
		where T:GroupResource
		where TSchema : GroupResourceSchemaBase
	{
		public GroupResourceAuthManager(ControllerContext controllercontext, HttpRequest request) 
			: base(controllercontext, request)
		{
		}

		protected override EPermissionLevel GetPermission(AccessContext context, T target)
		{
			if (context.User == null)
			{
				return EPermissionLevel.PUBLIC;
			}
			if (target.IsCreator(context))
			{
				return EPermissionLevel.OWNER;
			}
			if (target.IsAdmin(context.User, context))
			{
				return EPermissionLevel.ADMIN;
			}
			if (target.Members.IsAuthorised(context))
			{
				return EPermissionLevel.MEMBER;
			}
			return EPermissionLevel.USER;
		}

		protected override ResourceRequest CanCreate(AccessContext context, TSchema target)
		{
			if(context.User == null)
			{
				return ResourceRequest.ForbidRequest;
			}

#if DEBUG
			if(typeof(T) == typeof(Dodo.Rebellions.Rebellion))
			{
				return new ResourceRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.OWNER);
			}
#endif

			// User has a resource creation token, so we consume it and return ok
			var token = context.User.TokenCollection.GetTokens<ResourceCreationToken>()
				.FirstOrDefault(t => !t.IsRedeemed && t.Type == typeof(T).Name);
			if(token != null)
			{
				return new ResourceRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.OWNER, token);
			}

			// Test if user has admin on parent
			var parent = ResourceUtility.GetResourceByGuid(target.Parent) as GroupResource;
			if(parent == null)
			{
				return ResourceRequest.BadRequest;
			}
			if (!parent.IsAdmin(context.User, context))
			{
				return ResourceRequest.ForbidRequest;
			}
			return new ResourceRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.OWNER);
		}

		protected override ResourceRequest CanPost(AccessContext context, T target, string action = null)
		{
			if(action.Contains('?'))
			{
				action = action.Substring(action.IndexOf('?'));
			}
			switch(action)
			{
				case GroupResourceController<T, TSchema>.ADD_ADMIN:
					if(target.IsAdmin(context.User, context))
					{
						return new ResourceRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.ADMIN);
					}
					break;
				case GroupResourceController<T, TSchema>.JOIN_GROUP:
				case GroupResourceController<T, TSchema>.LEAVE_GROUP:
					if(context.User != null)
					{
						new ResourceRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.ADMIN);
					}
					break;
			}
			return ResourceRequest.UnauthorizedRequest;
		}
	}
}
