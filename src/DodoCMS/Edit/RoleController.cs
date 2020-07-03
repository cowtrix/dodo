using Dodo.Roles;
using Dodo.ViewModels;
using DodoResources.Roles;
using DodoResources.Sites;

namespace Dodo.Controllers.Edit
{
	public class RoleController : CrudController<Role, RoleSchema, RoleViewModel>
	{
		protected override AuthorizationService<Role, RoleSchema> AuthService =>
			new OwnedResourceAuthService<Role, RoleSchema>();
	}
}
