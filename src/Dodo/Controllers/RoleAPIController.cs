using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dodo.Roles;
using Dodo;
using Dodo.Sites;

namespace Dodo.Roles
{
	public class ApplicationModel
	{
		/// <summary>
		/// This should be the answer to Role.QuestionString
		/// </summary>
		public string Content { get; set; }
	}

	[Route(Dodo.DodoApp.API_ROOT + RootURL)]
	public class RoleAPIController : SearchableResourceController<Role, RoleSchema>
	{
		public const string RootURL = "role";

		protected override AuthorizationService<Role, RoleSchema> AuthService =>
			new RoleAuthService();

		protected RoleService RoleService =>
			new RoleService(Context, HttpContext, AuthService);

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] RoleSchema schema)
		{
			return (await PublicService.Create(schema)).ActionResult;
		}

		[HttpPost("{id}/" + RoleService.APPLY)]
		public async Task<IActionResult> Apply([FromRoute]string id, [FromBody] ApplicationModel application)
		{
			return (await RoleService.Apply(id, application)).ActionResult;
		}
	}
}
