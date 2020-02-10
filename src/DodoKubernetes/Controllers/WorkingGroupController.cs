using Common.Extensions;
using REST;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Dodo.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Dodo.WorkingGroups
{
	[Route(RootURL)]
	public class WorkingGroupController : GroupResourceController<WorkingGroup, WorkingGroupSchema>
	{
		public const string RootURL = "api/workinggroups";

		[HttpPost]
		[Authorize]
		public override async Task<IActionResult> Create([FromBody] WorkingGroupSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}