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
			var result = GroupService.UpdateAdmin(guid, id, permissionSet);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			var req = result as ResourceActionRequest;
			return Redirect($"~/edit/{typeof(T).Name}/{guid}");
		}

		[HttpPost("{id}/" + GroupResourceService<T, TSchema>.ADD_ADMIN)]
		public IActionResult AddAdministrator(string id, [FromForm]string newAdminIdentifier)
		{
			var result = GroupService.AddAdministrator(id, newAdminIdentifier);
			if(!result.IsSuccess)
			{
				return result.ActionResult;
			}
			return Redirect($"~/edit/{typeof(T).Name}/{id}");
		}

		[HttpGet("{id}/" + GroupResourceService<T, TSchema>.REMOVE_ADMIN)]
		public IActionResult RemoveAdministrator(string id, [FromQuery]string adminID)
		{
			var result = GroupService.RemoveAdministrator(id, adminID);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			return Redirect($"~/edit/{typeof(T).Name}/{id}");
		}
	}
}
