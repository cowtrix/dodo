using Dodo.LocationResources;
using Dodo.ViewModels;
using Dodo.Sites;

namespace Dodo.Controllers.Edit
{
	public class SiteController : CrudController<Site, SiteSchema, SiteViewModel>
	{
		protected override AuthorizationService<Site, SiteSchema> AuthService =>
			new GroupResourceAuthService<Site, SiteSchema>();
	}
}
