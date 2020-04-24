using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Dodo;
using System.Threading.Tasks;
using Dodo.Resources;

namespace DodoResources
{
	public abstract class SearchableResourceController<T, TSchema> : ResourceController<T, TSchema>
		where T : DodoResource
		where TSchema : ResourceSchemaBase
	{
		[HttpGet]
		public virtual async Task<IActionResult> IndexInternal(
			[FromQuery]DistanceFilter locationFilter,
			[FromQuery]DateFilter dateFilter,
			[FromQuery]StringFilter stringFilter,
			[FromQuery]ParentFilter parentFilter,
			string parent = null,
			int index = 0)
		{
			var req = VerifySearchRequest();
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			var resources = DodoResourceUtility.Search<T>(index, SearchController.ChunkSize, locationFilter, dateFilter, stringFilter, parentFilter);
			var view = resources.Select(rsc => rsc.GenerateJsonView(req.PermissionLevel, req.Requester.User, req.Requester.Passphrase));
			return Ok(view.ToList());
		}
	}
}
