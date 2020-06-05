using Dodo;
using Dodo.LocationResources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DodoResources.Sites
{
	[Route(DodoServer.DodoServer.API_ROOT + RootURL)]
	public class EventController : SearchableResourceController<Event, EventSchema>
	{
		public const string RootURL = "event";

		protected override AuthorizationService<Event, EventSchema> AuthManager => new SiteAuthManager<Event, EventSchema>();

		[HttpPost]
		public override async Task<IActionResult> Create([FromBody] EventSchema schema)
		{
			return await CreateInternal(schema);
		}
	}

	[Route(DodoServer.DodoServer.API_ROOT + RootURL)]
	public class SiteController : SearchableResourceController<Site, SiteSchema>
	{
		public const string RootURL = "site";

		protected override AuthorizationService<Site, SiteSchema> AuthManager => new SiteAuthManager<Site, SiteSchema>();

		[HttpPost]
		public override async Task<IActionResult> Create([FromBody] SiteSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}
