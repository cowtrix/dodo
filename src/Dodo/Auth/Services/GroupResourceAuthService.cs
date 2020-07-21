using Resources;
using System.Linq;
using Dodo.Users.Tokens;

namespace Dodo
{
	public class GroupResourceAuthService<T, TSchema> : AuthorizationService<T, TSchema>
		where T : class, IGroupResource
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
			if ((target is IOwnedResource owned) && owned.Parent.GetValue<IAdministratedResource>().IsAdmin(context.User, context, out _))
			{
				return EPermissionLevel.ADMIN;
			}
			if (target.IsMember(context))
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
			if (schema is OwnedResourceSchemaBase owned)
			{
				var parent = ResourceUtility.GetResourceByGuid(owned.Parent) as IAdministratedResource;
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
			if (action == IGroupResource.JOIN_GROUP || action == IGroupResource.LEAVE_GROUP)
			{
				return new ResourceActionRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.MEMBER);
			}
			// Everything below requires admin
			if (!(target is IOwnedResource owned) || !owned.Parent.GetValue<IAdministratedResource>().IsAdmin(context.User, context, out var permissionSet))
			{
				return ResourceRequestError.UnauthorizedRequest();
			}
			if (action == INotificationResource.ACTION_NOTIFICATION && permissionSet.CanManageAnnouncements)
			{
				return new ResourceActionRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.ADMIN, action);
			}
			return ResourceRequestError.UnauthorizedRequest();
		}
	}
}
