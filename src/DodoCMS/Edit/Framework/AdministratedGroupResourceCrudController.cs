using Microsoft.AspNetCore.Mvc;
using Resources;
using System;
using System.Text;
using System.Threading.Tasks;
using Common.Extensions;

namespace Dodo.Controllers.Edit
{
	public abstract class AdministratedGroupResourceCrudController<T, TSchema, TViewModel> : GroupResourceCrudController<T, TSchema, TViewModel>
		where T : class, IAdministratedResource, IGroupResource
		where TSchema : DescribedResourceSchemaBase, new()
		where TViewModel : class, IViewModel, new()
	{
		protected AdministratedGroupResourceService<T, TSchema> AdminService =>
			new AdministratedGroupResourceService<T, TSchema>(Context, HttpContext, AuthService);

		[HttpPost("{guid}/updateadmin")]
		public async Task<IActionResult> UpdateAdmin([FromRoute]string guid, [FromQuery]string id,
			[FromForm] AdministratorPermissionSet permissionSet)
		{
			var result = AdminService.UpdateAdmin(guid, id, permissionSet);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			var message = $"Successfully updated administrator.";
			return Redirect($"~/edit/{typeof(T).Name}/{guid}?message={Uri.EscapeDataString(message.EncodeBase64())}&tab=admin");
		}

		[HttpPost("{id}/" + AdministratedGroupResourceService<T, TSchema>.ADD_ADMIN)]
		public IActionResult AddAdministrator(string id, [FromForm]string newAdminIdentifier)
		{
			var result = AdminService.AddAdministrator(id, newAdminIdentifier);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			var message = $"Successfully added administrator.";
			return Redirect($"~/edit/{typeof(T).Name}/{id}?message={Uri.EscapeDataString(message.EncodeBase64())}&tab=admin");
		}

		[HttpGet("{id}/" + AdministratedGroupResourceService<T, TSchema>.REMOVE_ADMIN)]
		public IActionResult RemoveAdministrator(string id, [FromQuery]string adminID)
		{
			var result = AdminService.RemoveAdministrator(id, adminID);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			var message = $"Successfully removed administrator.";
			return Redirect($"~/edit/{typeof(T).Name}/{id}?message={Uri.EscapeDataString(message.EncodeBase64())}&tab=admin");
		}
	}
}
