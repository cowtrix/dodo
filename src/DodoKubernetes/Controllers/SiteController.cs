﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST;
using System.Threading.Tasks;

namespace Dodo.Sites
{
	[Route(RootURL)]
	public class SiteController : ObjectRESTController<Site, SiteSchema>
	{
		public const string RootURL = "api/sites";

		[HttpPost]
		[Authorize]
		public override async Task<IActionResult> Create([FromBody] SiteSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}