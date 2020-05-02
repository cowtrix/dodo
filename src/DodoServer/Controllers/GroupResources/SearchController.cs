using Microsoft.AspNetCore.Mvc;
using Dodo;
using System.Threading.Tasks;
using Resources;
using System.Linq;
using Common.Extensions;
using System;
using Dodo.Resources;
using Common.Config;

namespace DodoResources
{
	[Route(DodoServer.DodoServer.API_ROOT + RootURL)]
	public class SearchController : CustomController<DodoResource, ResourceSchemaBase>
	{
		public const string RootURL = "search";
		public static int ChunkSize => ConfigManager.GetValue($"{nameof(SearchController)}_SearchChunkSize", 100);

		[HttpGet]
		public virtual async Task<IActionResult> Index(
			[FromQuery]DistanceFilter locationFilter, 
			[FromQuery]DateFilter dateFilter,
			[FromQuery]StringFilter stringFilter,
			[FromQuery]ParentFilter parentFilter,
			[FromQuery]TypeFilter typeFilter,
			int index = 0, int chunkSize = int.MaxValue)
		{
			var req = VerifySearchRequest();
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			try
			{
				var resources = DodoResourceUtility.Search(index, Math.Min(chunkSize, ChunkSize), locationFilter, dateFilter, stringFilter, parentFilter, typeFilter)
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
