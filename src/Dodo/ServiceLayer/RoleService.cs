using Dodo.Roles;
using Dodo;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Resources;
using Microsoft.AspNetCore.Mvc;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

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
			var appRsc = role.Apply(Context, application, out var error);
			if (appRsc == default)
			{
				return ResourceRequestError.BadRequest(error);
			}
			ResourceManager.Update(role, rscLock);
			// Send them to the application
			return new ActionRequestResult(new RedirectResult($"{DodoApp.NetConfig.FullURI}/{nameof(RoleApplications).ToLowerInvariant()}/{appRsc.Guid}", false));
		}
	}
}
