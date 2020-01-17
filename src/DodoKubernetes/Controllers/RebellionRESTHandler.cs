using Common;
using Common.Extensions;
using REST;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Dodo.Utility;

namespace Dodo.Rebellions
{
	public class RebellionRESTHandler : GroupResourceRESTHandler<Rebellion>
	{
		protected override bool URLIsCreation(string url)
		{
			return url == Rebellion.ROOT + "/create";
		}

		protected bool URLIsList(string url)
		{
			return url == Rebellion.ROOT;
		}
	}
}
