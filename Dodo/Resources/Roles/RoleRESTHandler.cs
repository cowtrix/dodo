using Common;
using Common.Extensions;
using Dodo.Rebellions;
using Dodo.Resources;
using Dodo.Users;
using Dodo.WorkingGroups;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dodo.Roles
{
	public class RoleRESTHandler : DodoRESTHandler<Role>
	{
		protected override string CreationPostfix => Role.ROOT + "/create";

		public class CreationSchema : IRESTResourceSchema
		{
			public string Name = "";
			public string Mandate = "";
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema() { Name = "Test Working Group" };
		}

		protected override Role CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var user = DodoRESTServer.GetRequestOwner(request, out var passphrase);
			if(user == null)
			{
				throw HttpException.LOGIN;
			}
			var group = GetParentFromURL(request.Url);
			if(group == null)
			{
				throw new HttpException("Valid parent doesn't exist at " + request.Url, 404);
			}
			var newRole = new Role(user, passphrase, group, info);
			if (URLIsCreation(newRole.ResourceURL))
			{
				throw new Exception("Reserved Resource URL");
			}
			newRole.Verify();
			return newRole;
		}
	}
}
