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
	public class SearchController : CustomController<DodoResource, ResourceSchemaBase>
	{
		public const string RootURL = "search";
		public const int ChunkSize = 10;

		[HttpGet]
		public virtual async Task<IActionResult> Index(
			[FromQuery]DistanceFilter locationFilter, 
			[FromQuery]DateFilter dateFilter,
			[FromQuery]StringFilter stringFilter,
			[FromQuery]ParentFilter parentFilter,
			int index = 0)
		{
			var req = VerifySearchRequest();
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			try
			{
				var resources = DodoResourceUtility.Search(index, ChunkSize, locationFilter, dateFilter, stringFilter, parentFilter)
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
