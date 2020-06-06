using Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Dodo.LocalGroups;

namespace DodoResources.LocalGroups
{
	[Route(Dodo.Dodo.API_ROOT + RootURL)]
	public class LocalGroupController : GroupResourceController<LocalGroup, LocalGroupSchema>
	{
		public const string RootURL = "localgroup";

		[HttpPost]
		public override async Task<IActionResult> Create([FromBody] LocalGroupSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}
