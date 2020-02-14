using Dodo.Sites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST;
using System.Threading.Tasks;

namespace DodoResources.Sites
{
	[Route(RootURL)]
	public class SiteController : ObjectRESTController<Site, SiteSchema>
	{
		public const string RootURL = "api/sites";

		public SiteController(IAuthorizationService authorizationService) : base(authorizationService)
		{
		}

		[HttpPost]
		[Authorize]
		public override async Task<IActionResult> Create([FromBody] SiteSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}