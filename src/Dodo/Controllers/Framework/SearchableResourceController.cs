using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Dodo;
using System.Threading.Tasks;
using Dodo.DodoResources;
using System;

namespace Dodo
{
	public abstract class SearchableResourceController<T, TSchema> : CrudResourceAPIController<T, TSchema>
		where T : class, IPublicResource, IDodoResource
		where TSchema : ResourceSchemaBase
	{
		[HttpGet]
		public virtual async Task<IActionResult> IndexInternal(
			[FromQuery]DistanceFilter locationFilter,
			[FromQuery]DateFilter dateFilter,
			[FromQuery]StringFilter stringFilter,
			[FromQuery]ParentFilter parentFilter,
			int index = 0, int chunkSize = int.MaxValue)
		{
			try
			{
				var permissionLevel = Context.User == null ? EPermissionLevel.PUBLIC : EPermissionLevel.USER;
				var resources = DodoResourceUtility.Search<T>(index, Math.Min(chunkSize, SearchAPIController.ChunkSize), locationFilter, dateFilter, stringFilter, parentFilter)
					.Select(rsc => rsc.GenerateJsonView(permissionLevel, Context.User, Context.Passphrase));
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
