using Common;
using Common.Extensions;
using REST;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Dodo.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Dodo.Rebellions
{
	[Route(RootURL)]
	public class RebellionController : GroupResourceController<Rebellion, RebellionSchema>
	{
		public const string RootURL = "api/rebellions";
	}
}
