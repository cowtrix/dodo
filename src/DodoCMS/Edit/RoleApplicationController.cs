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
using Resources.Security;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Dodo.RoleApplications
{
	[SecurityHeaders]
	[ApiController]
	[Route(RoleApplication.ROOT_URL)]
	public class RoleApplicationController : CustomController
	{
		protected RoleApplicationViewModel ViewModel(RoleApplication rsc, object requester, Resources.Security.Passphrase passphrase) =>
			rsc.CopyByValue<RoleApplicationViewModel>(requester, passphrase);
		protected AuthorizationService<RoleApplication, RoleApplicationSchema> AuthService => new RoleApplicationAuthService();
		protected CrudResourceServiceBase<RoleApplication, RoleApplicationSchema> PublicService =>
			new CrudResourceServiceBase<RoleApplication, RoleApplicationSchema>(Context, HttpContext, AuthService);

		[Route("{id}")]
		public virtual async Task<IActionResult> ViewApplication([FromRoute] string id)
		{
			Response.Headers.Add("Content-Security-Policy", $"frame-ancestors 'self' {DodoApp.NetConfig.FullURI}");
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
			object requester = null;
			Passphrase pass = default;
			IAsymmCapableResource parentRsc = null;
			if (actionResult.PermissionLevel == EPermissionLevel.ADMIN)
			{
				var rscRef = new ResourceReference<IAsymmCapableResource>(rsc.Parent.Parent);   // the working group
				requester = rscRef;
				parentRsc = rscRef.GetValue();
				pass = parentRsc.GetPrivateKey(Context);
			}
			else if (actionResult.PermissionLevel == EPermissionLevel.OWNER)
			{
				requester = Context.User.CreateRef<IAsymmCapableResource>();
				pass = Context.Passphrase;
			}
			var model = ViewModel(rsc, requester, pass);
			if (model.Data == null)
			{
				var rawData = rsc.Data.GetValueByGroup(actionResult.AccessContext, parentRsc);
				model.Data = rawData.CopyByValue<RoleApplicationViewModel.RoleApplicationDataViewModel>(null, default);
			}
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

		[HttpPost("{id}/" + RoleApplication.MESSAGE)]
		public virtual async Task<IActionResult> SendMessage([FromRoute] string id, [FromForm] Guid salt,
			[FromForm] string content, [FromForm] bool adminOnly = false, [FromQuery] bool header = true)
		{
			if (string.IsNullOrEmpty(content) || salt == default)
			{
				return Redirect($"/{RoleApplication.ROOT_URL}/{id}");
			}
			var result = AuthService.IsAuthorised(Context, id, EHTTPRequestType.POST, RoleApplication.MESSAGE);
			if (!result.IsSuccess)
			{
				return result.ActionResult;
			}
			var resourceReq = result as ResourceActionRequest;
			using var rscLock = new ResourceLock(resourceReq.Result);
			var roleApp = rscLock.Value as RoleApplication;
			if (resourceReq.PermissionLevel == EPermissionLevel.ADMIN)
			{
				var groupRef = new ResourceReference<IAsymmCapableResource>(roleApp.Parent.Parent);
				var group = groupRef.GetValue();
				var requester = groupRef;
				var roleAppData = roleApp.Data.GetValueByGroup(resourceReq.AccessContext, group);
				if (roleAppData == null)
				{
					return BadRequest();
				}
				if (!roleAppData.Messages.Any(m => m.Guid == salt)) // Prevent double post
				{
					roleAppData.Messages.Add(
					new Message(resourceReq.AccessContext, content,
						resourceReq.PermissionLevel == EPermissionLevel.OWNER, adminOnly, salt));
					roleApp.Data.SetValueByGroup(roleAppData, resourceReq.AccessContext, group);
				}
			}
			else if (resourceReq.PermissionLevel == EPermissionLevel.OWNER)
			{
				object requester = resourceReq.AccessContext.User.CreateRef<IAsymmCapableResource>();
				Passphrase passphrase = resourceReq.AccessContext.Passphrase;
				if (!roleApp.Data.TryGetValue(requester, passphrase, out var dataObj) ||
					!(dataObj is RoleApplicationData roleAppData))
				{
					return BadRequest();
				}
				if (!roleAppData.Messages.Any(m => m.Guid == salt)) // Prevent double post
				{
					roleAppData.Messages.Add(new Message(resourceReq.AccessContext, content, resourceReq.PermissionLevel == EPermissionLevel.OWNER, false, salt));
					roleApp.Data.SetValue(roleAppData, resourceReq.AccessContext.User.CreateRef<IAsymmCapableResource>(), resourceReq.AccessContext.Passphrase);
				}
			}
			ResourceUtility.GetManager<RoleApplication>().Update(roleApp, rscLock);
			return Redirect($"/{RoleApplication.ROOT_URL}/{id}?{(header ? "" : "header=false")}");
		}
	}
}
