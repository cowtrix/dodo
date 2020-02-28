using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dodo.Roles;

namespace DodoResources.Roles
{
	[Route(RootURL)]
	public class RoleController : ObjectRESTController<Role, RoleSchema>
	{
		public const string RootURL = "api/roles";

		[HttpPost]
		public override async Task<IActionResult> Create([FromBody] RoleSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}
