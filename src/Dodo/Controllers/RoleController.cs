using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dodo.Roles;
using Dodo;

namespace DodoResources.Roles
{

	[Route(Dodo.Dodo.API_ROOT + RootURL)]
	public class RoleController : ResourceController<Role, RoleSchema>
	{
		public const string RootURL = "role";

		protected override AuthorizationService<Role, RoleSchema> AuthManager => new RoleAuthManager();

		[HttpPost]
		public override async Task<IActionResult> Create([FromBody] RoleSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}
