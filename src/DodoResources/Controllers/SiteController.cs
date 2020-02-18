using Dodo.Sites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DodoResources.Sites
{
	[Route(RootURL)]
	public class SiteController : GroupResourceController<Site, SiteSchema>
	{
		public const string RootURL = "api/sites";

		[HttpPost]
		public override async Task<IActionResult> Create([FromBody] SiteSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}