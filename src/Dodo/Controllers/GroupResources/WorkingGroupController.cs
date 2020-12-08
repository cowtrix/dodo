using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dodo.WorkingGroups
{
	[Route(Dodo.DodoApp.API_ROOT + RootURL)]
	public class WorkingGroupController : AdministratedGroupResourceAPIController<WorkingGroup, WorkingGroupSchema>
	{
		public const string RootURL = "workinggroup";

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] WorkingGroupSchema schema)
		{
			return (await PublicService.Create(schema)).ActionResult;
		}
	}
}
