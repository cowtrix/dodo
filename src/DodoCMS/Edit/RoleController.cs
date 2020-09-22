using Dodo.Roles;
using Dodo.ViewModels;

namespace Dodo.Controllers.Edit
{
	public class RoleController : CrudController<Role, RoleSchema, RoleViewModel>
	{
		protected override AuthorizationService<Role, RoleSchema> AuthService =>
			new RoleAuthService();
	}
}
