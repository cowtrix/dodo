using Resources;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using Dodo.Users;
using System.Linq;

namespace DodoResources
{
	public class GroupResourceAuthManager<T, TSchema> : AuthorizationManager<T, TSchema>
		where T:GroupResource
		where TSchema : GroupResourceSchemaBase
	{
		protected override EPermissionLevel GetPermission(AccessContext context, T target)
		{
			if (context.User == null)
			{
				return EPermissionLevel.PUBLIC;
			}
			if (context.User.GUID == target.Creator.Guid)
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
			// User has a resource creation token, so we consume it and return ok
			var token = context.User.Tokens.GetTokens<ResourceCreationToken>()
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
	}
}
