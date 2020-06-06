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

namespace DodoResources
{
	public abstract class GroupResourceController<T, TSchema> : SearchableResourceController<T, TSchema> 
		where T : GroupResource
		where TSchema : OwnedResourceSchemaBase
	{
		protected GroupResourceService<T, TSchema> GroupService => new GroupResourceService<T, TSchema>(Context, HttpContext);

		[HttpPost("{id}/" + GroupResourceService<T, TSchema>.ADD_ADMIN)]
		public IActionResult AddAdministrator(Guid id, [FromBody]string newAdminIdentifier)
		{
			var result = GroupService.AddAdministrator(id, newAdminIdentifier);
			return result.Result;
		}

		[HttpPost("{id}/" + GroupResourceService<T, TSchema>.JOIN_GROUP)]
		public IActionResult JoinGroup(Guid id)
		{
			var result = GroupService.JoinGroup(id);
			return result.Result;
		}

		[HttpPost("{id}/" + GroupResourceService<T, TSchema>.LEAVE_GROUP)]
		public IActionResult LeaveGroup(Guid id)
		{
			var result = GroupService.LeaveGroup(id);
			return result.Result;
		}
	}
}
