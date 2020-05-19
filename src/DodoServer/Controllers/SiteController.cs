using Dodo;
using Dodo.Sites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DodoResources.Sites
{
	[Route(DodoServer.DodoServer.API_ROOT + RootURL)]
	public class SiteController : SearchableResourceController<Site, SiteSchema>
	{
		public const string RootURL = "site";

		protected override AuthorizationService<Site, SiteSchema> AuthManager => new SiteAuthManager();

		[HttpPost]
		public override async Task<IActionResult> Create([FromBody] SiteSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}
