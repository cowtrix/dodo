using Microsoft.AspNetCore.Mvc;
using Dodo;
using System.Threading.Tasks;
using Resources;
using System.Linq;
using Common.Extensions;
using System;

namespace DodoResources
{
	[Route(RootUrl)]
	public class SearchController : CustomController<DodoResource, DodoResourceSchemaBase>
	{
		public const string RootUrl = "search";

		[HttpGet]
		public virtual async Task<IActionResult> Index(
			[FromQuery]DistanceFilter locationFilter, [FromQuery]DateFilter dateFilter)
		{
			var req = VerifyRequest();
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			try
			{
				var resources = DodoResourceUtility.Get(rsc => locationFilter.Filter(rsc) && dateFilter.Filter(rsc))
					.Transpose(x => locationFilter.Mutate(x))
					.Transpose(x => dateFilter.Mutate(x));
				var guids = resources.Select(rsc => rsc.GUID).ToList();
				return Ok(guids);
			}
			catch (Exception e)
			{
#if DEBUG
				return BadRequest(e.Message);
#else
				return BadRequest();
#endif
			}
		}
	}
}
