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
using Microsoft.AspNetCore.Authorization;
using Dodo;

namespace DodoResources
{
	public abstract class GroupResourceController<T, TSchema> : ObjectRESTController<T, TSchema> 
		where T : GroupResource
		where TSchema : DodoResourceSchemaBase
	{
		public const string ADD_ADMIN = "addadmin";
		public const string JOIN_GROUP = "join";
		public const string LEAVE_GROUP = "leave";

		public GroupResourceController(IAuthorizationService authorizationService) : base(authorizationService)
		{
		}

		[HttpPost("{id}/" + ADD_ADMIN)]
		[Authorize]
		public IActionResult AddAdministrator(Guid resourceID, [FromBody]string newAdminIdentifier)
		{
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

		public const char FilterVarSeperatorChar = '+';
		public struct FilterModel
		{
			public string latlong;
			public double distance;
			public string startdate;
			public string enddate;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Index(FilterModel? filter)
		{
			if(filter.HasValue)
			{
				var filterVars = filter.Value;
				if (!typeof(T).IsAssignableFrom(typeof(ILocationalResource)))
				{
					return BadRequest("Filter invalid for this type of resource");
				}
				var latlong = filterVars.latlong.Split(FilterVarSeperatorChar).Select(x => double.Parse(x))
					.Transpose(x => new GeoLocation(x.ElementAt(0), x.ElementAt(1)));
				var startDate = string.IsNullOrEmpty(filterVars.startdate) ? DateTime.MinValue : DateTime.Parse(filterVars.startdate);
				var endDate = string.IsNullOrEmpty(filterVars.enddate) ? DateTime.MaxValue : DateTime.Parse(filterVars.enddate);

				//return Ok(ResourceManager.Get(rsc => rsc.Start))
			}
			return Ok(ResourceManager.Get(x => true).Select(rsc => rsc.GUID));
		}
	}
}
