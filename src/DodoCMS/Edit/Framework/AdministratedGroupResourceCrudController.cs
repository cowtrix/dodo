using Microsoft.AspNetCore.Mvc;
using Resources;
using System.Threading.Tasks;

namespace Dodo.Controllers.Edit
{
	public abstract class AdministratedGroupResourceCrudController<T, TSchema, TViewModel> : GroupResourceCrudController<T, TSchema, TViewModel>
		where T : class, IAdministratedResource, IGroupResource
		where TSchema : DescribedResourceSchemaBase, new()
		where TViewModel : class, IViewModel, new()
	{
		protected AdministratedGroupResourceService<T, TSchema> AdminService =>
			new AdministratedGroupResourceService<T, TSchema>(Context, HttpContext, AuthService);

		[HttpPost]
		[Route("{guid}/updateadmin")]
		public async Task<IActionResult> UpdateAdmin([FromRoute]string guid, [FromQuery]string id,
			[FromForm] AdministratorPermissionSet permissionSet)
		{
			var result = AdminService.UpdateAdmin(guid, id, permissionSet);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			var req = result as ResourceActionRequest;
			return Redirect($"~/edit/{typeof(T).Name}/{guid}");
		}

		[HttpPost("{id}/" + AdministratedGroupResourceService<T, TSchema>.ADD_ADMIN)]
		public IActionResult AddAdministrator(string id, [FromForm]string newAdminIdentifier)
		{
			var result = AdminService.AddAdministrator(id, newAdminIdentifier);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			return Redirect($"~/edit/{typeof(T).Name}/{id}");
		}

		[HttpGet("{id}/" + AdministratedGroupResourceService<T, TSchema>.REMOVE_ADMIN)]
		public IActionResult RemoveAdministrator(string id, [FromQuery]string adminID)
		{
			var result = AdminService.RemoveAdministrator(id, adminID);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			return Redirect($"~/edit/{typeof(T).Name}/{id}");
		}
	}
}
