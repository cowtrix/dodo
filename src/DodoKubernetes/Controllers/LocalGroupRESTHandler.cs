using Common;
using Common.Extensions;
using REST;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Dodo.Utility;

namespace Dodo.LocalGroups
{
	public class LocalGroupRESTHandler : GroupResourceRESTHandler<LocalGroup>
	{
		protected bool URLIsList(string url)
		{
			return url == LocalGroup.ROOT;
		}

		protected override bool URLIsCreation(string url)
		{
			return url == LocalGroup.ROOT + "/create";
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new LocalGroup.CreationSchema("Name of local group", "Public description of local group", new GeoLocation());
		}

		protected override LocalGroup CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			
		}
	}
}
