using Dodo.LocationResources;
using Dodo.ViewModels;
using DodoResources.Sites;

namespace Dodo.Controllers.Edit
{
	public class SiteController : CrudController<Site, SiteSchema, SiteViewModel>
	{
		protected override AuthorizationService<Site, SiteSchema> AuthService =>
			new OwnedResourceAuthService<Site, SiteSchema>();
	}
}
