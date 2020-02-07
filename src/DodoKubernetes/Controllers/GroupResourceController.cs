using System.Collections.Generic;
using Common;
using Common.Extensions;
using REST.Security;
using Dodo.Users;
using Dodo.Utility;
using Newtonsoft.Json;
using REST;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Common.Security;
using System.Net;
using System.Linq;
using System;

namespace Dodo
{
	public abstract class GroupResourceController<T, TSchema> : ObjectRESTController<T, TSchema> 
		where T : GroupResource
		where TSchema : DodoResourceSchemaBase
	{
		public const string ADD_ADMIN = "addadmin";
		public const string JOIN_GROUP = "join";
		public const string LEAVE_GROUP = "leave";

		[HttpPost("{id}/" + ADD_ADMIN)]
		public IActionResult AddAdministrator(Guid resourceID, [FromBody]string newAdminIdentifier)
		{
			var resource = ResourceManager.GetSingle(x => x.GUID == resourceID);
			if (resource == null)
			{
				return NotFound();
			}
			var context = Request.GetRequestOwner();
			if (!IsAuthorised(context, resource, Request.MethodEnum(), out var permissionLevel))
			{
				return Forbid();
			}
			if (permissionLevel < EPermissionLevel.ADMIN)
			{
				throw HttpException.FORBIDDEN;
			}
			var userManager = ResourceUtility.GetManager<User>();
			User targetUser = null;
			if(Guid.TryParse(newAdminIdentifier, out var newAdminGuid))
			{
				targetUser = userManager.GetSingle(x => x.GUID == newAdminGuid);
			}
			else if(ValidationExtensions.EmailIsValid(newAdminIdentifier))
			{
				targetUser = userManager.GetSingle(x => x.PersonalData.Email == newAdminIdentifier);
			}
			if(resource.AddAdmin(context, targetUser))
			{
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("{id}/" + JOIN_GROUP)]
		public IActionResult JoinGroup(Guid id)
		{
			var context = Request.GetRequestOwner();
			using var resourceLock = new ResourceLock(id);
			var target = resourceLock.Value as GroupResource;
			if (target == null)
			{
				return NotFound();
			}
			target.Members.Add(context.User, context.Passphrase);
			ResourceManager.Update(target, resourceLock);
			return Ok();
		}

		[HttpPost("{id}/" + LEAVE_GROUP)]
		public IActionResult LeaveGroup(Guid id)
		{
			var context = Request.GetRequestOwner();
			using var resourceLock = new ResourceLock(id);
			var target = resourceLock.Value as GroupResource;
			if (target == null)
			{
				return NotFound();
			}
			target.Members.Remove(context.User, context.Passphrase);
			ResourceManager.Update(target, resourceLock);
			return HttpBuilder.OK();
		}

		[HttpGet]
		public IActionResult Index()
		{
			return Ok(ResourceManager.Get(x => true).Select(rsc => rsc.GUID));
		}
	}
}
