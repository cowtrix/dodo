using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dodo.Roles;
using Dodo;
using Dodo.Sites;

namespace Dodo.Roles
{
	[Route(Dodo.DodoApp.API_ROOT + RootURL)]
	public class RoleAPIController : SearchableResourceController<Role, RoleSchema>
	{
		public const string RootURL = "role";

		protected override AuthorizationService<Role, RoleSchema> AuthService =>
			new RoleAuthService();

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] RoleSchema schema)
		{
			return (await PublicService.Create(schema)).ActionResult;
		}
	}
}
