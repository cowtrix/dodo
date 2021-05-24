using Dodo.LocationResources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dodo.Sites
{
	[Route(Dodo.DodoApp.API_ROOT + RootURL)]
	public class EventAPIController : GroupResourceAPIController<Event, EventSchema>
	{
		public const string RootURL = "event";

		protected override AuthorizationService<Event, EventSchema> AuthService => 
			new AdministratedGroupResourceAuthService<Event, EventSchema>();

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] EventSchema schema)
		{
			var result = await PublicService.Create(schema);
			return result.ActionResult;
		}
	}
}
