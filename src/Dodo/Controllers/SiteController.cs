using Dodo;
using Dodo.LocationResources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DodoResources.Sites
{

	[Route(Dodo.Dodo.API_ROOT + RootURL)]
	public class SiteController : SearchableResourceController<Site, SiteSchema>
	{
		public const string RootURL = "site";

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] SiteSchema schema)
		{
			var result = await PublicService.Create(schema);
			return result.Result;
		}
	}
}
