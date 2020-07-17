using Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Dodo.LocalGroups;

namespace Dodo.LocalGroups
{
	[Route(Dodo.DodoApp.API_ROOT + RootURL)]
	public class LocalGroupAPIController : AdministratedGroupResourceAPIController<LocalGroup, LocalGroupSchema>
	{
		public const string RootURL = "localgroup";
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] LocalGroupSchema schema)
		{
			return (await PublicService.Create(schema)).ActionResult;
		}
	}
}
