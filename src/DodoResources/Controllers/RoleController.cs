using Common.Extensions;
using Resources;
using Microsoft.AspNetCore.Http;
using System;
using Dodo.Utility;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Dodo.Roles;

namespace DodoResources.Roles
{
	[Route(RootURL)]
	public class RoleController : ObjectRESTController<Role, RoleSchema>
	{
		public const string RootURL = "api/roles";

		public RoleController(IAuthorizationService authorizationService) : base(authorizationService)
		{
		}

		[HttpPost]
		[Authorize]
		public override async Task<IActionResult> Create([FromBody] RoleSchema schema)
		{
			return await CreateInternal(schema);
		}
	}
}
