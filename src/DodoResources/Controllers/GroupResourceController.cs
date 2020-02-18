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
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using Dodo;
using System.Threading.Tasks;

namespace DodoResources
{
	public abstract class GroupResourceController<T, TSchema> : ObjectRESTController<T, TSchema> 
		where T : GroupResource
		where TSchema : DodoResourceSchemaBase
	{
		public const string ADD_ADMIN = "addadmin";
		public const string JOIN_GROUP = "join";
		public const string LEAVE_GROUP = "leave";

		[HttpPost("{id}/" + ADD_ADMIN)]
		[Authorize]
		public IActionResult AddAdministrator(Guid resourceID, [FromBody]string newAdminIdentifier)
		{
			LogRequest();
			var resource = ResourceManager.GetSingle(x => x.GUID == resourceID);
			if (resource == null)
			{
				return NotFound();
			}
			var context = User.GetRequestOwner();
			if (!IsAuthorised(context, resource, Request.MethodEnum(), out var permissionLevel))
			{
				return Forbid();
			}
			if (permissionLevel < EPermissionLevel.ADMIN)
			{
				return Unauthorized();
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
		[Authorize]
		public IActionResult JoinGroup(Guid id)
		{
			LogRequest();
			var context = User.GetRequestOwner();
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
		[Authorize]
		public IActionResult LeaveGroup(Guid id)
		{
			LogRequest();
			var context = User.GetRequestOwner();
			using var resourceLock = new ResourceLock(id);
			var target = resourceLock.Value as GroupResource;
			if (target == null)
			{
				return NotFound();
			}
			target.Members.Remove(context.User, context.Passphrase);
			ResourceManager.Update(target, resourceLock);
			return Ok();
		}

		[HttpGet]
		[AllowAnonymous]
		public virtual async Task<IActionResult> IndexInternal(
			[FromQuery]DistanceFilter locationFilter, [FromQuery]DateFilter dateFilter)
		{
			LogRequest();
			try
			{
				var allrsc = ResourceManager.Get(x => true).ToList();
				var resources = ResourceManager.Get(rsc => locationFilter.Filter(rsc) && dateFilter.Filter(rsc))
					.Transpose(x => locationFilter.Mutate(x))
					.Transpose(x => dateFilter.Mutate(x));
				var guids = resources.Select(rsc => rsc.GUID).ToList();
				return Ok(guids);
			}
			catch(Exception e)
			{
#if DEBUG
				return BadRequest(e.Message);
#else
				return BadRequest();
#endif
			}
		}
	}
}
