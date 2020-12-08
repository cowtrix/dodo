using Microsoft.AspNetCore.Mvc;
using Resources;
using System.Threading.Tasks;

namespace Dodo.RoleApplications
{
	[Route(Dodo.DodoApp.API_ROOT + RootURL)]
	public class RoleApplicationAPIController : CustomController
	{
		public const string RootURL = "roleapplication";

		protected AuthorizationService<RoleApplication, RoleApplicationSchema> AuthService =>
			new RoleApplicationAuthService();

		protected RoleApplicationService RoleApplicationService =>
			new RoleApplicationService(Context, HttpContext, AuthService);

		[HttpPost("{id}/" + RoleApplicationService.APPLY)]
		public async Task<IActionResult> Apply([FromRoute] string id, [FromBody]string application)
		{
			return RoleApplicationService.Apply(id, application).ActionResult;
		}
	}
}
