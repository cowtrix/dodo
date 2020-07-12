using Dodo;
using Dodo.LocationResources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dodo.Sites
{

	[Route(Dodo.DodoApp.API_ROOT + RootURL)]
	public class SiteAPIController : SearchableResourceController<Site, SiteSchema>
	{
		public const string RootURL = "site";

		protected override AuthorizationService<Site, SiteSchema> AuthService =>
			new OwnedResourceAuthService<Site, SiteSchema>();

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] SiteSchema schema)
		{
			var result = await PublicService.Create(schema);
			return result.ActionResult;
		}
	}
}
