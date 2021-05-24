using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dodo;
using Dodo.Models;
using Dodo.Users;
using Dodo.Users.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Resources;

namespace Dodo.Controllers.Edit
{
	public interface IViewModel
	{
		uint Revision { get; }
		Guid Guid { get; }
	}

	[Route("edit/[controller]")]
	public abstract class CrudController<T, TSchema, TViewModel> : CustomController
	where T : class, IDodoResource, IPublicResource
	where TSchema : ResourceSchemaBase, new()
	where TViewModel : class, IViewModel, new()
	{
		protected virtual CrudResourceServiceBase<T, TSchema> CrudService => new CrudResourceServiceBase<T, TSchema>(Context, HttpContext, AuthService);
		protected abstract AuthorizationService<T, TSchema> AuthService { get; }
		protected TViewModel ViewModel(T rsc) => rsc.CopyByValue<TViewModel>(Context.User.CreateRef(), Context.Passphrase);

		[Route("create")]
		public virtual IActionResult Create([FromQuery] string parent = null)
		{
			if (Context.User == null)
			{
				// redirect to login
				return Forbid();
			}
			var tokens = Context.User.TokenCollection.GetAllTokens<ResourceCreationToken>(Context, EPermissionLevel.ADMIN, Context.User)
				.Where(t => t.ResourceType == typeof(T).Name && !t.IsRedeemed);
			if (!tokens.Any() && Context.User.TokenCollection.GetSingleToken<SysadminToken>(Context, EPermissionLevel.ADMIN, Context.User) == null)
			{
				return Unauthorized("You must request permission to create this resource.");
			}
			var schema = new TSchema();
			if (schema is OwnedResourceSchemaBase owned)
			{
				IRESTResource rsc;
				if (Guid.TryParse(parent, out var guid))
				{
					rsc = ResourceUtility.GetResourceByGuid(guid);
				}
				else
				{
					rsc = ResourceUtility.GetResourceBySlug(parent);
				}
				if (rsc == null)
				{
					return BadRequest($"No parent resource found with ID {parent}");
				}
				owned.ParentID = rsc.Guid.ToString();
				ViewData["Parent"] = rsc.Guid;
			}
			schema.OnView(Context.User, Context.Passphrase);
			return View(schema);
		}

		[HttpPost]
		[Route("create")]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Create([FromForm] TSchema schema)
		{
			if (Context.User == null)
			{
				// redirect to login
				return Forbid();
			}
			try
			{
				if (!ModelState.IsValid)
				{
					return View(schema);
				}
				var request = await CrudService.Create(schema);
				if (!request.IsSuccess)
				{
					var error = request as ResourceRequestError;
					ModelState.AddModelError("", error.Message ?? error.ActionResult.ToString());
					return View(schema);
				}
				var creationReq = request as ResourceCreationRequest;
				return RedirectToAction(nameof(Edit), new { id = creationReq.Result.Slug });
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(schema);
			}
		}

		[Route("{id}")]
		public virtual async Task<IActionResult> Edit([FromRoute] string id)
		{
			if (Context.User == null)
			{
				// redirect to login
				return Forbid();
			}
			var result = AuthService.IsAuthorised(Context, id, EHTTPRequestType.PATCH);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			var actionResult = result as ResourceActionRequest;
			var rsc = actionResult.Result as T;
			ViewData["Permission"] = actionResult.PermissionLevel;
			if (rsc is INotificationResource notificationResource)
			{
				ViewData["Notifications"] = notificationResource.GetNotifications(Context, actionResult.PermissionLevel);
			}
			if (rsc is IAdministratedResource admin)
			{
				admin.IsAdmin(Context.User, Context, out var permissions);
				ViewData["permissions"] = permissions;
			}
			else if (rsc is IOwnedResource owned)
			{
				owned.Parent.GetValue<IAdministratedResource>(true).IsAdmin(Context.User, Context, out var permissions);
				ViewData["permissions"] = permissions;
			}
			if (rsc is Roles.Role role)
			{
				var asymm = role.Parent.GetValue<IAsymmCapableResource>(true);
				ViewData["applications"] = role.Applications.ToDictionary(kvp => kvp.Key,
					kvp => kvp.Value);
			}
			ViewData[RESPONSE_VIEWDATA] = result.Message;
			var model = ViewModel(rsc);
			return View(model);
		}

		[HttpPost]
		[Route("{id}")]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Edit([FromRoute] string id, [FromForm] TViewModel modified)
		{
			if (Context.User == null)
			{
				// redirect to login
				return Forbid();
			}
			try
			{
				if (!ModelState.IsValid)
				{
					var errors = ModelState.Values.Where(v => v.Errors.Any()).ToList();
					ModelState.AddModelError("", "Error updating the resource");
					return await Edit(id);
				}
				var result = await CrudService.Update(id, modified);
				if (!result.IsSuccess)
				{
					var error = result as ResourceRequestError;
					ModelState.AddModelError("", error.Message);
					return await Edit(id);
				}
				return RedirectToAction(nameof(Edit));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return await Edit(id);
			}
		}

		[HttpGet]
		[Route("{id}/delete")]
		public virtual async Task<IActionResult> Delete([FromRoute]string id)
		{
			if (Context.User == null)
			{
				// redirect to login
				return Forbid();
			}
			var result = await CrudService.Get(id);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			var getResult = result as ResourceActionRequest;
			var model = ViewModel(getResult.Result as T);
			return View(model);
		}

		[HttpPost]
		[Route("{id}/delete")]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Delete(TViewModel view)
		{
			if (Context.User == null)
			{
				// redirect to login
				return Forbid();
			}
			try
			{
				var request = (await CrudService.Delete(view.Guid.ToString()));
				return Redirect(Dodo.DodoApp.NetConfig.FullURI);
			}
			catch (Exception e)
			{
				ModelState.AddModelError("Unable to delete resource", e.Message);
				return RedirectToAction(nameof(Edit));
			}
		}

		[HttpPost("notifications/{id}/new")]
		public virtual async Task<IActionResult> PostNotification([FromRoute]string id, [FromForm]NotificationModel notification)
		{
			var result = await CrudService.AddNotification(id, notification);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			return RedirectToAction(nameof(Edit), new { id = id });
		}

		[HttpGet("notifications/{id}/delete")]
		public virtual async Task<IActionResult> DeleteNotification([FromRoute]string id, [FromQuery]Guid notification)
		{
			var result = await CrudService.DeleteNotification(id, notification);
			if(!result.IsSuccess)
			{
				return result.ActionResult;
			}
			return RedirectToAction(nameof(Edit), new { id = id });
		}

		public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			base.OnActionExecuting(actionContext);
			ViewData["auth"] = AuthService;
		}
	}
}
