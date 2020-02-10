using REST;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Dodo.LocalGroups
{
	[Route(RootURL)]
	public class LocalGroupController : GroupResourceController<LocalGroup, LocalGroupSchema>
	{
		public const string RootURL = "api/localgroups";

		[HttpPost]
		[Authorize]
		public override async Task<IActionResult> Create([FromBody] LocalGroupSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}
