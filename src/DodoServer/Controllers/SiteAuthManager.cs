using Dodo;
using Dodo.Sites;
using Resources;

namespace DodoResources.Sites
{
	public class SiteAuthManager : AuthorizationService<Site, SiteSchema>
	{
		public SiteAuthManager() : base()
		{
		}

		protected override ResourceRequest CanCreate(AccessContext context, SiteSchema target)
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
