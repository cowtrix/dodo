using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Resources;
using System.Linq;
using System;
using Dodo.DodoResources;
using Common.Config;
using Common;
using System.Diagnostics;

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
			//using var _= new FunctionTimer(TimeSpan.FromSeconds(5));
			try
			{
				var sw = Stopwatch.StartNew();
				var permissionLevel = Context.User == null ? EPermissionLevel.PUBLIC : EPermissionLevel.USER;
				var resources = DodoResourceUtility.Search(index, Math.Min(chunkSize, ChunkSize), false,
					locationFilter, dateFilter, stringFilter, parentFilter, typeFilter).ToList();
				var results = resources.Select(rsc => rsc.GenerateJsonView(permissionLevel, Context.User, Context.Passphrase)).ToList();
				Logger.Debug($"Search took {sw.Elapsed}");
				return Ok(results);
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
