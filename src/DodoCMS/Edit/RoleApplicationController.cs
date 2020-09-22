using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dodo.Models;
using Dodo.Users.Tokens;
using System.Linq;
using Dodo.Rebellions;
using System.Collections.Generic;
using System;
using Dodo.ViewModels;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Dodo.RoleApplications
{
	[SecurityHeaders]
	[ApiController]
	[Route("roleapplication")]
	public class RoleApplicationController : CustomController
	{
		protected RoleApplicationViewModel ViewModel(RoleApplication rsc) => rsc.CopyByValue<RoleApplicationViewModel>(Context.User.CreateRef<IAsymmCapableResource>(), Context.Passphrase);
		protected AuthorizationService<RoleApplication, RoleApplicationSchema> AuthService => new RoleApplicationAuthService();
		protected CrudResourceServiceBase<RoleApplication, RoleApplicationSchema> PublicService =>
			new CrudResourceServiceBase<RoleApplication, RoleApplicationSchema>(Context, HttpContext, AuthService);

		[Route("{id}")]
		public virtual async Task<IActionResult> ViewApplication([FromRoute] string id)
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
			var rsc = actionResult.Result as RoleApplication;
			ViewData["Permission"] = actionResult.PermissionLevel;
			var model = ViewModel(rsc);
			return View(model);
		}

		[HttpPost]
		[Route("{id}")]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> ViewApplication([FromRoute] string id, [FromForm] RoleApplicationViewModel modified)
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
					ModelState.AddModelError("Save Error", "Error updating the resource");
					return await ViewApplication(id);
				}
				var result = await PublicService.Update(id, modified);
				if (!result.IsSuccess)
				{
					return result.ActionResult;
				}
				return RedirectToAction(nameof(ViewApplication));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return await ViewApplication(id);
			}
		}
	}
}
