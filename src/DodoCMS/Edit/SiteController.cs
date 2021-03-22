using Dodo.LocationResources;
using Dodo.ViewModels;
using Dodo.Sites;

namespace Dodo.Controllers.Edit
{
	public class SiteController : AdministratedGroupResourceCrudController<Site, SiteSchema, SiteViewModel>
	{
		protected override AuthorizationService<Site, SiteSchema> AuthService =>
			new AdministratedGroupResourceAuthService<Site, SiteSchema>();
	}
}
