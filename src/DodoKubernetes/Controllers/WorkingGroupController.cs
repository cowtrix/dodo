using Common.Extensions;
using REST;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Dodo.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Dodo.WorkingGroups
{
	[Route(RootURL)]
	public class WorkingGroupController : GroupResourceController<WorkingGroup, WorkingGroupSchema>
	{
		public const string RootURL = "api/workinggroups";
	}
}