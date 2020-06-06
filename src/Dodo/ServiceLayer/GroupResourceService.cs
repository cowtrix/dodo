using System.Collections.Generic;
using Common.Extensions;
using Dodo.Users;
using Resources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Dodo;

namespace DodoResources
{
	public class GroupResourceService<T, TSchema> : ResourceServiceBase<T, TSchema>
		where T : GroupResource
		where TSchema : OwnedResourceSchemaBase
	{
		public const string ADD_ADMIN = "addadmin";
		public const string JOIN_GROUP = "join";
		public const string LEAVE_GROUP = "leave";

		public GroupResourceService(AccessContext context, HttpContext httpContext) : base(context, httpContext)
		{
		}

		public override AuthorizationService<T, TSchema> AuthManager => new GroupResourceAuthManager<T, TSchema>();

		public IRequestResult AddAdministrator(Guid id, string newAdminIdentifier)
		{
			var reqResult = VerifyRequest(id, EHTTPRequestType.POST, ADD_ADMIN);
			if (!reqResult.IsSuccess)
			{
				return reqResult;
			}
			var req = (ResourceActionRequest)reqResult;
			var userManager = UserManager;
			User targetUser = null;
			if(Guid.TryParse(newAdminIdentifier, out var newAdminGuid))
			{
				targetUser = userManager.GetSingle(x => x.Guid == newAdminGuid);
			}
			else if(ValidationExtensions.EmailIsValid(newAdminIdentifier))
			{
				targetUser = userManager.GetSingle(x => x.PersonalData.Email == newAdminIdentifier) ??
					UserService.CreateTemporaryUser(newAdminIdentifier);
			}
			var resource = reqResult.Result as T;
			if(resource.AddAdmin(req.AccessContext, targetUser))
			{
				return new OkRequestResult();
			}
			return ResourceRequestError.BadRequest();
		}

		public IRequestResult JoinGroup(Guid id)
		{
			var reqResult = VerifyRequest(id, EHTTPRequestType.POST, ADD_ADMIN);
			if (!reqResult.IsSuccess)
			{
				return reqResult;
			}
			var req = (ResourceActionRequest)reqResult;
			using var resourceLock = new ResourceLock(id);
			var target = resourceLock.Value as T;
			target.Members.Add(req.AccessContext.User, req.AccessContext.Passphrase);
			ResourceManager.Update(target, resourceLock);
			return new OkRequestResult();
		}

		[HttpPost("{id}/" + LEAVE_GROUP)]
		public IRequestResult LeaveGroup(Guid id)
		{
			var reqResult = VerifyRequest(id, EHTTPRequestType.POST, ADD_ADMIN);
			if (!reqResult.IsSuccess)
			{
				return reqResult;
			}
			var req = (ResourceActionRequest)reqResult;
			using var resourceLock = new ResourceLock(id);
			var target = resourceLock.Value as T;
			target.Members.Remove(req.AccessContext.User, req.AccessContext.Passphrase);
			ResourceManager.Update(target, resourceLock);
			return new OkRequestResult();
		}
	}
}
