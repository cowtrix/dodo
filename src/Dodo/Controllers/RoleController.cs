using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dodo.Roles;
using Dodo;

namespace DodoResources.Roles
{

	[Route(Dodo.Dodo.API_ROOT + RootURL)]
	public class RoleController : PublicResourceAPIController<Role, RoleSchema>
	{
		public const string RootURL = "role";

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] RoleSchema schema)
		{
			return (await PublicService.Create(schema)).Result;
		}
	}
}
