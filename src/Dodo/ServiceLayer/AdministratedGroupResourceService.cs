using Common.Extensions;
using Dodo.Users;
using Resources;
using Microsoft.AspNetCore.Http;
using System;

namespace Dodo
{
	public class AdministratedGroupResourceService<T, TSchema> : ResourceServiceBase<T, TSchema>
		where T : class, IAdministratedResource
		where TSchema : DescribedResourceSchemaBase
	{
		public const string ADD_ADMIN = "addadmin";
		public const string REMOVE_ADMIN = "removeadmin";
		public const string UPDATE_ADMIN = "updateadmin";

		public AdministratedGroupResourceService(AccessContext context, HttpContext httpContext, AuthorizationService<T, TSchema> auth)
			: base(context, httpContext, auth)
		{
		}

		public IRequestResult AddAdministrator(string resourceIdentifier, string newAdminIdentifier)
		{
			var reqResult = VerifyRequest(resourceIdentifier, EHTTPRequestType.POST, ADD_ADMIN);
			if (!reqResult.IsSuccess)
			{
				return reqResult;
			}
			if (string.IsNullOrEmpty(newAdminIdentifier))
			{
				return ResourceRequestError.BadRequest("No user specified");
			}
			var req = (ResourceActionRequest)reqResult;
			var userManager = UserManager;
			User targetUser = null;
			if (Guid.TryParse(newAdminIdentifier, out var newAdminGuid))
			{
				targetUser = userManager.GetSingle(x => x.Guid == newAdminGuid);
			}
			else if (ValidationExtensions.EmailIsValid(newAdminIdentifier))
			{
				// TODO: figure out WHY this is necessary
				newAdminIdentifier = newAdminIdentifier.Replace(" ", "+");
				targetUser = userManager.GetSingle(x => x.PersonalData.Email == newAdminIdentifier) ??
					UserService.CreateTemporaryUser(newAdminIdentifier);
			}
			else
			{
				targetUser = userManager.GetSingle(x => x.Slug == newAdminIdentifier);
			}
			if (targetUser == null)
			{
				return ResourceRequestError.NotFoundRequest();
			}
			using var rscLock = new ResourceLock(req.Result);
			var resource = rscLock.Value as IAdministratedResource;
			if (resource.AddNewAdmin(req.AccessContext, targetUser))
			{
				ResourceManager.Update(resource, rscLock);
				return new OkRequestResult();
			}
			return ResourceRequestError.BadRequest();
		}

		public IRequestResult RemoveAdministrator(string resourceIdentifier, string adminIdentifier)
		{
			var reqResult = VerifyRequest(resourceIdentifier, EHTTPRequestType.POST, REMOVE_ADMIN);
			if (!reqResult.IsSuccess)
			{
				return reqResult;
			}
			if (string.IsNullOrEmpty(adminIdentifier))
			{
				return ResourceRequestError.BadRequest("No user specified");
			}
			var req = (ResourceActionRequest)reqResult;
			User targetUser = null;
			if (Guid.TryParse(adminIdentifier, out var newAdminGuid))
			{
				targetUser = UserManager.GetSingle(x => x.Guid == newAdminGuid);
			}
			else
			{
				targetUser = UserManager.GetSingle(x => x.Slug == adminIdentifier);
			}
			if (targetUser == null)
			{
				return ResourceRequestError.NotFoundRequest();
			}
			using var rscLock = new ResourceLock(req.Result);
			var resource = rscLock.Value as IAdministratedResource;
			if (resource.RemoveAdmin(req.AccessContext, targetUser))
			{
				ResourceManager.Update(resource, rscLock);
				return new OkRequestResult();
			}
			return ResourceRequestError.BadRequest();
		}

		public IRequestResult UpdateAdmin(string resourceID, string adminID, AdministratorPermissionSet permissionSet)
		{
			if (Context.User == null)
			{
				// redirect to login
				return ResourceRequestError.ForbidRequest();
			}
			if (!typeof(IAdministratedResource).IsAssignableFrom(typeof(T)))
			{
				return ResourceRequestError.BadRequest();
			}
			User targetUser = null;
			if (Guid.TryParse(adminID, out var newAdminGuid))
			{
				targetUser = UserManager.GetSingle(x => x.Guid == newAdminGuid);
			}
			else
			{
				targetUser = UserManager.GetSingle(x => x.Slug == adminID);
			}
			if (targetUser == null)
			{
				return ResourceRequestError.NotFoundRequest();
			}
			var request = AuthService.IsAuthorised(Context, resourceID, EHTTPRequestType.PATCH);
			if (!request.IsSuccess)
			{
				return ResourceRequestError.UnauthorizedRequest();
			}
			var actionReq = request as ResourceActionRequest;
			using var rscLock = new ResourceLock(actionReq.Result);
			var rsc = rscLock.Value as IAdministratedResource;
			if (!rsc.UpdateAdmin(Context, targetUser, permissionSet))
			{
				return ResourceRequestError.BadRequest();
			}
			ResourceManager.Update(rsc, rscLock);
			return new OkRequestResult();
		}

	}
}
