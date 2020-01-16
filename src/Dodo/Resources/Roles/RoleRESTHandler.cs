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


		public class CreationSchema : IRESTResourceSchema
		{
			public string Name = "";
			public string PublicDescription = "";
		}

		protected override IRESTResourceSchema GetCreationSchema()
		{
			return new CreationSchema() { Name = "Test Working Group" };
		}

		protected override Role CreateFromSchema(HttpRequest request, IRESTResourceSchema schema)
		{
			var info = (CreationSchema)schema;
			var user = request.GetRequestOwner(out var passphrase);
			if(user == null)
			{
				throw HttpException.LOGIN;
			}
			var group = GetParentFromURL(request.Path);
			if(group == null)
			{
				throw new HttpException("Valid parent doesn't exist at " + request.Path, HttpStatusCode.NotFound);
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
