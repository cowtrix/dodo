using Dodo.Roles;
using Dodo;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Resources;
using Microsoft.AspNetCore.Mvc;

namespace Dodo.Roles
{
	public class RoleService : ResourceServiceBase<Role, RoleSchema>
	{
		public const string APPLY = "apply";

		public RoleService(AccessContext context, HttpContext httpContext, AuthorizationService<Role, RoleSchema> authService) : 
			base(context, httpContext, authService)
		{
		}

		public async Task<IRequestResult> Apply(string id, ApplicationModel application)
		{
			var req = VerifyRequest(id, EHTTPRequestType.POST, APPLY);
			if(!req.IsSuccess)
			{
				return req;
			}
			var actionReq = req as ResourceActionRequest;
			var role = actionReq.Result as Role;
			using var rscLock = new ResourceLock(role);
			role = rscLock.Value as Role;
			if (!role.Apply(Context, application))
			{
				return ResourceRequestError.BadRequest();
			}
			ResourceManager.Update(role, rscLock);
			return new OkRequestResult("Application successful");
		}
	}
}
