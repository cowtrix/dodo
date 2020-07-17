using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dodo
{
	public abstract class AdministratedGroupResourceAPIController<T, TSchema> : GroupResourceAPIController<T, TSchema>
		where T : AdministratedGroupResource
		where TSchema : DescribedResourceSchemaBase
	{
		[HttpPost]
		[Route("{guid}/" + AdministratedGroupResourceService<T, TSchema>.UPDATE_ADMIN)]
		public async Task<IActionResult> UpdateAdmin([FromRoute]string guid, [FromQuery]string id,
			[FromBody] AdministratorPermissionSet permissionSet)
		{
			var result = AdminService.UpdateAdmin(guid, id, permissionSet);
			return result.ActionResult;
		}

		[HttpGet]
		[Route("{guid}/" + AdministratedGroupResourceService<T, TSchema>.REMOVE_ADMIN)]
		public async Task<IActionResult> RemoveAdmin([FromRoute]string guid, [FromQuery]string id)
		{
			var result = AdminService.RemoveAdministrator(guid, id);
			return result.ActionResult;
		}

		[HttpPost("{id}/" + AdministratedGroupResourceService<T, TSchema>.ADD_ADMIN)]
		public IActionResult AddAdministrator(string id, [FromBody]string newAdminIdentifier)
		{
			var result = AdminService.AddAdministrator(id, newAdminIdentifier);
			return result.ActionResult;
		}

		protected AdministratedGroupResourceService<T, TSchema> AdminService =>
			new AdministratedGroupResourceService<T, TSchema>(Context, HttpContext, AuthService);
		protected override AuthorizationService<T, TSchema> AuthService => 
			new AdministratedGroupResourceAuthService<T, TSchema>();

	}
}
