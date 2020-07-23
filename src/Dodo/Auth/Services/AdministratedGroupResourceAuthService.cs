using Resources;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using Dodo.Users;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Dodo.Users.Tokens;
using MongoDB.Bson.Serialization.Serializers;

namespace Dodo
{
	public class AdministratedGroupResourceAuthService<T, TSchema> : AuthorizationService<T, TSchema>
		where T : class, IAdministratedResource
		where TSchema : DescribedResourceSchemaBase
	{
		public AdministratedGroupResourceAuthService() : base()
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
			if (target is IGroupResource group && group.IsMember(context))
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
				var parent = owned.GetParent() as T;
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
			if (context.User == null)
			{
				return ResourceRequestError.ForbidRequest();
			}
			if (action.Contains('?'))
			{
				action = action.Substring(action.IndexOf('?'));
			}
			if(action == IGroupResource.JOIN_GROUP || action == IGroupResource.LEAVE_GROUP)
			{
				return new ResourceActionRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.MEMBER);
			}
			// Everything below requires admin
			if(!target.IsAdmin(context.User, context, out var permissionSet))
			{
				return ResourceRequestError.UnauthorizedRequest();
			}
			if ((action == AdministratedGroupResourceService<T, TSchema>.ADD_ADMIN && permissionSet.CanAddAdmin) ||
				(action == AdministratedGroupResourceService<T, TSchema>.UPDATE_ADMIN && permissionSet.CanChangePermissions) ||
				(action == AdministratedGroupResourceService<T, TSchema>.REMOVE_ADMIN && permissionSet.CanRemoveAdmin) ||
				(action == INotificationResource.ACTION_NOTIFICATION && permissionSet.CanManageAnnouncements))
			{
				return new ResourceActionRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.ADMIN, action);
			}
			return ResourceRequestError.UnauthorizedRequest();
		}
	}
}
