using Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Dodo.LocalGroups;

namespace DodoResources.LocalGroups
{
	[Route(RootURL)]
	public class LocalGroupController : GroupResourceController<LocalGroup, LocalGroupSchema>
	{
		public const string RootURL = "api/localgroups";

		public LocalGroupController(IAuthorizationService authorizationService) : base(authorizationService)
		{
		}

		[HttpPost]
		[Authorize(Roles = PermissionLevel.ADMIN)]
		public override async Task<IActionResult> Create([FromBody] LocalGroupSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}
