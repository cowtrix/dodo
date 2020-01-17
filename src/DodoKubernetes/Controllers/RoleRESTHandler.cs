using Common.Extensions;
using REST;
using Microsoft.AspNetCore.Http;
using System;
using Dodo.Utility;
using System.Net;

namespace Dodo.Roles
{
	public class RoleRESTHandler : DodoRESTHandler<Role>
	{
		protected override string CreationPostfix => Role.ROOT + "/create";

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new Role.CreationSchema() { Name = "Test Working Group" };
		}

		protected override Role CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			
		}
	}
}
