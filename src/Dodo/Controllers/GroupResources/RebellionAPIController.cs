using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dodo.Rebellions
{
	[Route(Dodo.DodoApp.API_ROOT + RootURL)]
	public class RebellionAPIController : AdministratedGroupResourceAPIController<Rebellion, RebellionSchema>
	{
		public const string RootURL = "rebellion";

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] RebellionSchema schema)
		{
			var result = await PublicService.Create(schema);
			return result.ActionResult;
		}
	}
}
