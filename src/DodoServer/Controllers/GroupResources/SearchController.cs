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
			var req = VerifySearchRequest();
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			try
			{
				var resources = DodoResourceUtility.Get(rsc => locationFilter.Filter(rsc) && dateFilter.Filter(rsc))
					.Transpose(x => locationFilter.Mutate(x))
					.Transpose(x => dateFilter.Mutate(x));
				return Ok(DodoJsonViewUtility.GenerateJsonViewEnumerable(resources, req.PermissionLevel, 
					req.Requester.User, req.Requester.Passphrase));
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

		protected ResourceRequest VerifySearchRequest()
		{
			if (!Context.Challenge())
			{
				return ResourceRequest.ForbidRequest;
			}
			if(Context.User == null)
			{
				return new ResourceRequest(Context, null, EHTTPRequestType.GET, EPermissionLevel.PUBLIC);
			}
			return new ResourceRequest(Context, null, EHTTPRequestType.GET, EPermissionLevel.MEMBER);
		}
	}
}
