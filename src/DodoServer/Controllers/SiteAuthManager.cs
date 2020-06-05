using Dodo;
using Dodo.LocationResources;
using Resources;

namespace DodoResources.Sites
{
	public class SiteAuthManager<T, TSchema> : AuthorizationService<T, TSchema>
		where T : LocationResourceBase
		where TSchema : LocationResourceSchema
	{
		public SiteAuthManager() : base()
		{
		}

		protected override ResourceRequest CanCreate(AccessContext context, TSchema target)
		{
			var parent = ResourceUtility.GetResourceByGuid(target.Parent) as GroupResource;
			if (parent == null)
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
