using Microsoft.AspNetCore.Mvc;
using Dodo;
using System.Threading.Tasks;
using Resources;
using System.Linq;
using Common.Extensions;
using System;
using Dodo.Resources;

namespace DodoResources
{
	[Route(DodoServer.DodoServer.API_ROOT + RootURL)]
	public class SearchController : CustomController<DodoResource, DodoResourceSchemaBase>
	{
		public const string RootURL = "search";
		private const int ChunkSize = 10;

		[HttpGet]
		public virtual async Task<IActionResult> Index(
			[FromQuery]DistanceFilter locationFilter, 
			[FromQuery]DateFilter dateFilter,
			[FromQuery]StringFilter stringFilter,
			int index = 0)
		{
			var req = VerifySearchRequest();
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			try
			{
				var resources = DodoResourceUtility.Search(locationFilter, dateFilter, stringFilter, index, ChunkSize)
					.Select(rsc => rsc.GenerateJsonView(req.PermissionLevel, req.Requester.User, req.Requester.Passphrase));
				return Ok(resources.ToList());
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
