using Dodo.Users;
using DodoResources;
using Microsoft.AspNetCore.Mvc;
using Resources;
using System;
using System.Threading.Tasks;

namespace Dodo.Controllers.Edit
{
	public abstract class GroupResourceCrudController<T, TSchema, TViewModel> : CrudController<T, TSchema, TViewModel>
		where T : GroupResource
		where TSchema : DescribedResourceSchemaBase, new()
		where TViewModel : class, IViewModel, new()
	{
		protected GroupResourceService<T, TSchema> GroupService =>
			new GroupResourceService<T, TSchema>(Context, HttpContext, AuthService);

		[HttpPost]
		[Route("{guid}/updateadmin")]
		public async Task<IActionResult> UpdateAdmin([FromRoute]string guid, [FromQuery]string id, 
			[FromForm] AdministratorPermissionSet permissionSet)
		{
			return GroupService.UpdateAdmin(guid, id, permissionSet).ActionResult;
		}

		[HttpPost("{id}/" + GroupResourceService<T, TSchema>.ADD_ADMIN)]
		public IActionResult AddAdministrator(string id, [FromBody]string newAdminIdentifier)
		{
			var result = GroupService.AddAdministrator(id, newAdminIdentifier);
			return result.ActionResult;
		}
	}
}
