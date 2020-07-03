using System.Collections.Generic;
using Common;
using Common.Extensions;
using Resources.Security;
using Dodo.Users;
using Dodo.Utility;
using Newtonsoft.Json;
using Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Common.Security;
using System.Net;
using System;
using Microsoft.AspNetCore.Authorization;
using Dodo;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DodoResources
{
	public abstract class GroupResourceAPIController<T, TSchema> : SearchableResourceController<T, TSchema>
		where T : GroupResource
		where TSchema : DescribedResourceSchemaBase
	{
		protected GroupResourceService<T, TSchema> GroupService =>
			new GroupResourceService<T, TSchema>(Context, HttpContext, AuthService);
		protected override AuthorizationService<T, TSchema> AuthService => new GroupResourceAuthService<T, TSchema>();

		[HttpPost]
		[Route("{guid}/updateadmin")]
		public async Task<IActionResult> UpdateAdmin([FromRoute]string guid, [FromQuery]string id,
			[FromBody] AdministratorPermissionSet permissionSet)
		{
			var result = GroupService.UpdateAdmin(guid, id, permissionSet);
			return result.ActionResult;
		}

		[HttpPost("{id}/" + GroupResourceService<T, TSchema>.ADD_ADMIN)]
		public IActionResult AddAdministrator(string id, [FromBody]string newAdminIdentifier)
		{
			var result = GroupService.AddAdministrator(id, newAdminIdentifier);
			return result.ActionResult;
		}

		[HttpPost("{id}/" + GroupResourceService<T, TSchema>.JOIN_GROUP)]
		public IActionResult JoinGroup(string id)
		{
			var result = GroupService.JoinGroup(id);
			return result.ActionResult;
		}

		[HttpPost("{id}/" + GroupResourceService<T, TSchema>.LEAVE_GROUP)]
		public IActionResult LeaveGroup(string id)
		{
			var result = GroupService.LeaveGroup(id);
			return result.ActionResult;
		}
	}
}
