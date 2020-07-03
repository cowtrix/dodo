using Resources;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using Dodo.Users;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Dodo.Users.Tokens;

namespace DodoResources
{
	public class GroupResourceAuthService<T, TSchema> : AuthorizationService<T, TSchema>
		where T : GroupResource
		where TSchema : DescribedResourceSchemaBase
	{
		public GroupResourceAuthService() : base()
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
			if (target.IsAdmin(context.User, context, out _))
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

			// User has a resource creation token with this type, so we consume it and return ok
			var token = context.User.TokenCollection.GetAllTokens<ResourceCreationToken>(context, EPermissionLevel.OWNER, context.User)
				.FirstOrDefault(t => !t.IsRedeemed && t.ResourceType == typeof(T).Name);
			if (token != null || context.User.TokenCollection.GetSingleToken<SysadminToken>(context, EPermissionLevel.OWNER, context.User) != null)
			{
				return new ResourceCreationRequest(context, schema, token);
			}

			// Does the user have permission to create a child of the parent?
			if(schema is OwnedResourceSchemaBase owned)
			{
				var parent = ResourceUtility.GetResourceByGuid(owned.Parent) as GroupResource;
				if (parent == null)
				{
					return ResourceRequestError.BadRequest();
				}
				if (!parent.IsAdmin(context.User, context, out var permissionSet)
					|| !permissionSet.CanCreateChildObjects)
				{
					return ResourceRequestError.UnauthorizedRequest();
				}
				return new ResourceCreationRequest(context, schema);
			}
			return ResourceRequestError.UnauthorizedRequest();
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
					if (target.IsAdmin(context.User, context, out var permissionSet) || !permissionSet.CanAddAdmin)
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
