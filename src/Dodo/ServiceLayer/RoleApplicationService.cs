using Dodo.Roles;
using Microsoft.AspNetCore.Http;
using Resources;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Dodo.RoleApplications
{
	public class RoleApplicationService : ResourceServiceBase<RoleApplication, RoleApplicationSchema>
	{
		public const string APPLY = "apply";

		private IResourceManager<Role> RoleResourceManager => ResourceUtility.GetManager<Role>();

		public RoleApplicationService(AccessContext context, HttpContext httpContext, AuthorizationService<RoleApplication, RoleApplicationSchema> authService) :
			base(context, httpContext, authService)
		{
		}

		public IRequestResult Apply(string id, string application)
		{
			var role = ResourceUtility.GetManager<Role>().GetSingle(r => r.Guid.ToString() == id || r.Slug == id);
			var schema = new RoleApplicationSchema($"Application for {role.Name}", role, application);
			var req = VerifyRequest(schema);
			if (!req.IsSuccess)
			{
				return req;
			}
			var actionReq = req as ResourceCreationRequest;
			var factory = new RoleApplicationFactory();
			var appRsc = factory.CreateTypedObject(actionReq);
			// Send them to the application
			return new RedirectRequestResult($"{DodoApp.NetConfig.FullURI}/{nameof(RoleApplication).ToLowerInvariant()}/{appRsc.Guid}?header=false", false);
		}
	}
}
