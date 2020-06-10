using Dodo.LocationResources;
using DodoResources.Sites;

namespace Dodo.Controllers.Edit
{
	public class SiteController : CrudController<Site, SiteSchema>
	{
		protected override AuthorizationService<Site, SiteSchema> AuthService =>
			new OwnedResourceAuthService<Site, SiteSchema>();
	}
}
