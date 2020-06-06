using Resources;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using Dodo.Users;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Dodo.Users.Tokens;

namespace DodoResources
{
	public class GroupResourceAuthManager<T, TSchema> : AuthorizationService<T, TSchema>
		where T : GroupResource
		where TSchema : OwnedResourceSchemaBase
	{
		public GroupResourceAuthManager() : base()
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

		protected override IRequestResult CanCreate(AccessContext context, TSchema schema)
		{
			if (context.User == null)
			{
				return ResourceRequestError.ForbidRequest();
			}

#if DEBUG
			if (
				typeof(T) == typeof(Dodo.Rebellions.Rebellion) ||
				typeof(T) == typeof(Dodo.LocalGroups.LocalGroup) ||
				typeof(T) == typeof(Dodo.WorkingGroups.WorkingGroup)
			)
			{
				return new ResourceCreationRequest(context, schema, EHTTPRequestType.POST, EPermissionLevel.OWNER);
			}
#endif

			// User has a resource creation token, so we consume it and return ok
			var token = context.User.TokenCollection.GetAllTokens<ResourceCreationToken>(context)
				.FirstOrDefault(t => !t.IsRedeemed && t.ResourceType == typeof(T).Name);
			if (token != null)
			{
				return new ResourceCreationRequest(context, schema, EHTTPRequestType.POST, EPermissionLevel.OWNER, token);
			}

			// Test if user has admin on parent
			var parent = ResourceUtility.GetResourceByGuid(schema.Parent) as GroupResource;
			if (parent == null)
			{
				return ResourceRequestError.BadRequest();
			}
			if (!parent.IsAdmin(context.User, context))
			{
				return ResourceRequestError.ForbidRequest();
			}
			return new ResourceCreationRequest(context, schema, EHTTPRequestType.POST, EPermissionLevel.OWNER);
		}

		protected override IRequestResult CanPost(AccessContext context, T target, string action = null)
		{
			if (action.Contains('?'))
			{
				action = action.Substring(action.IndexOf('?'));
			}
			switch (action)
			{
				case GroupResourceService<T, TSchema>.ADD_ADMIN:
					if (target.IsAdmin(context.User, context))
					{
						return new ResourceActionRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.ADMIN);
					}
					break;
				case GroupResourceService<T, TSchema>.JOIN_GROUP:
				case GroupResourceService<T, TSchema>.LEAVE_GROUP:
					if (context.User != null)
					{
						return new ResourceActionRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.ADMIN);
					}
					return ResourceRequestError.ForbidRequest();
			}
			return ResourceRequestError.UnauthorizedRequest();
		}
	}
}
