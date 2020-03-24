using Common.Extensions;
using Resources;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Dodo.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Dodo.WorkingGroups;

namespace DodoResources.WorkingGroups
{
	[Route(DodoServer.DodoServer.API_ROOT + RootURL)]
	public class WorkingGroupController : GroupResourceController<WorkingGroup, WorkingGroupSchema>
	{
		public const string RootURL = "workinggroups";

		[HttpPost]
		public override async Task<IActionResult> Create([FromBody] WorkingGroupSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}
