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

namespace Dodo.WorkingGroups
{
	[Route(Dodo.DodoApp.API_ROOT + RootURL)]
	public class WorkingGroupController : AdministratedGroupResourceAPIController<WorkingGroup, WorkingGroupSchema>
	{
		public const string RootURL = "workinggroup";

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] WorkingGroupSchema schema)
		{
			return (await PublicService.Create(schema)).ActionResult;
		}
	}
}
