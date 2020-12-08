using Microsoft.AspNetCore.Mvc;
using Dodo;
using System.Threading.Tasks;
using Resources;
using System.Linq;
using Common.Extensions;
using System;
using Dodo.DodoResources;
using Common.Config;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Dodo
{
	[Route(Dodo.DodoApp.API_ROOT + RootURL)]
	public class SearchAPIController : CustomController
	{
		public const string RootURL = "search";
		public static int ChunkSize => ConfigManager.GetValue($"{nameof(SearchAPIController)}_SearchChunkSize", 25);

		[HttpGet]
		public virtual async Task<IActionResult> Index(
			[FromQuery]DistanceFilter locationFilter,
			[FromQuery]DateFilter dateFilter,
			[FromQuery]StringFilter stringFilter,
			[FromQuery]ParentFilter parentFilter,
			[FromQuery]TypeFilter typeFilter,
			int index = 0, int chunkSize = int.MaxValue)
		{
			try
			{
				var permissionLevel = Context.User == null ? EPermissionLevel.PUBLIC : EPermissionLevel.USER;
				var resources = DodoResourceUtility.Search(index, Math.Min(chunkSize, ChunkSize), locationFilter, dateFilter, stringFilter, parentFilter, typeFilter)
					.Select(rsc => rsc.GenerateJsonView(permissionLevel, Context.User, Context.Passphrase));
				return Ok(resources.ToList());
			}
			catch (Exception e)
			{
				Common.Logger.Exception(e, "Exception in search");
#if DEBUG
				return BadRequest(e.Message);
#else
				return BadRequest();
#endif
			}
		}
	}
}
