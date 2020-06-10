using Dodo.Roles;
using DodoResources.Roles;

namespace Dodo.Controllers.Edit
{
	public class RoleController : CrudController<Role, RoleSchema>
	{
		protected override AuthorizationService<Role, RoleSchema> AuthService =>
			new RoleAuthService();
	}
}
