using Microsoft.AspNetCore.Mvc;
using REST;

namespace Dodo.Sites
{
	[Route(RootURL)]
	public class SiteController : ObjectRESTController<Site, SiteSchema>
	{
		public const string RootURL = "api/sites";

		[HttpPost]
		public override IActionResult Create([FromBody] SiteSchema schema)
		{
			return CreateInternal(schema);
		}
	}
}