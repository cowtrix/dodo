using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dodo.Roles;
using Dodo;

namespace DodoResources.Roles
{

	[Route(RootURL)]
	public class RoleController : ResourceController<Role, RoleSchema>
	{
		public const string RootURL = "api/roles";

		protected override AuthorizationManager<Role, RoleSchema> AuthManager => 
			new RoleAuthManager(this.ControllerContext, Request);

		[HttpPost]
		public override async Task<IActionResult> Create([FromBody] RoleSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}
