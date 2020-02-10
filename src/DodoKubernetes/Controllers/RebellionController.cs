using Common;
using Common.Extensions;
using REST;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Dodo.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Dodo.Rebellions
{
	[Route(RootURL)]
	public class RebellionController : GroupResourceController<Rebellion, RebellionSchema>
	{
		public const string RootURL = "api/rebellions";

		[HttpPost]
		[Authorize]
		public override async Task<IActionResult> Create([FromBody] RebellionSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}
