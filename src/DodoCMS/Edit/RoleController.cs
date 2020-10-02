using Dodo.Roles;
using Dodo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dodo.Controllers.Edit
{
	public class RoleController : CrudController<Role, RoleSchema, RoleViewModel>
	{
		protected override AuthorizationService<Role, RoleSchema> AuthService =>
			new RoleAuthService();

		public override Task<IActionResult> Edit([FromRoute] string id)
		{
			Response.Headers.Add("Content-Security-Policy", $"frame-ancestors 'self' {DodoApp.NetConfig.FullURI}");
			return base.Edit(id);
		}
	}
}
