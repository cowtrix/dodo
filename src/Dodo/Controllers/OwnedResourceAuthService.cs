using Dodo;
using Dodo.LocationResources;
using Resources;

namespace DodoResources.Sites
{
	public class OwnedResourceAuthService<T, TSchema> : AuthorizationService<T, TSchema>
		where T : IOwnedResource
		where TSchema : OwnedResourceSchemaBase
	{
		public OwnedResourceAuthService() : base()
		{
		}

		protected override IRequestResult CanCreate(AccessContext context, TSchema target)
		{
			var parent = ResourceUtility.GetResourceByGuid(target.Parent) as GroupResource;
			if (parent == null)
			{
				return ResourceRequestError.BadRequest();
			}
			if (!parent.IsAdmin(context.User, context))
			{
				return ResourceRequestError.ForbidRequest();
			}
			return new ResourceCreationRequest(context, target, EHTTPRequestType.POST, EPermissionLevel.OWNER);
		}
	}
}
