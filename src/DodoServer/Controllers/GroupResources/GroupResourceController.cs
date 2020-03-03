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
	public abstract class GroupResourceController<T, TSchema> : ResourceController<T, TSchema> 
		where T : GroupResource
		where TSchema : GroupResourceSchemaBase
	{
		public const string ADD_ADMIN = "addadmin";
		public const string JOIN_GROUP = "join";
		public const string LEAVE_GROUP = "leave";

		protected override AuthorizationManager<T, TSchema> AuthManager => 
			new GroupResourceAuthManager<T, TSchema>(this.ControllerContext, Request);

		[HttpPost("{id}/" + ADD_ADMIN)]
		public IActionResult AddAdministrator(Guid id, [FromBody]string newAdminIdentifier)
		{
			var req = VerifyRequest(id);
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			var userManager = UserManager;
			User targetUser = null;
			if(Guid.TryParse(newAdminIdentifier, out var newAdminGuid))
			{
				targetUser = userManager.GetSingle(x => x.GUID == newAdminGuid);
			}
			else if(ValidationExtensions.EmailIsValid(newAdminIdentifier))
			{
				targetUser = userManager.GetSingle(x => x.PersonalData.Email == newAdminIdentifier);
			}
			var resource = req.Resource as T;
			if(resource.AddAdmin(req.Requester, targetUser))
			{
				return Ok();
			}
			return BadRequest();
		}

		[HttpPost("{id}/" + JOIN_GROUP)]
		public IActionResult JoinGroup(Guid id)
		{
			var req = VerifyRequest(id);
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			using var resourceLock = new ResourceLock(id);
			var target = resourceLock.Value as T;
			target.Members.Add(req.Requester.User, req.Requester.Passphrase);
			ResourceManager.Update(target, resourceLock);
			return Ok();
		}

		[HttpPost("{id}/" + LEAVE_GROUP)]
		public IActionResult LeaveGroup(Guid id)
		{
			var req = VerifyRequest(id);
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			using var resourceLock = new ResourceLock(id);
			var target = resourceLock.Value as T;
			target.Members.Remove(req.Requester.User, req.Requester.Passphrase);
			ResourceManager.Update(target, resourceLock);
			return Ok();
		}

		[HttpGet]
		public virtual async Task<IActionResult> IndexInternal(
			[FromQuery]DistanceFilter locationFilter, [FromQuery]DateFilter dateFilter)
		{
			var req = VerifyRequest();
			if (!req.IsSuccess)
			{
				return req.Error;
			}
			try
			{
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
