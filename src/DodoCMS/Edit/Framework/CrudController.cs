using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dodo;
using Dodo.Users.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;

namespace Dodo.Controllers.Edit
{
	[Route("edit/[controller]")]
	public abstract class CrudController<T, TSchema> : CustomController
		where T : DodoResource, IPublicResource
		where TSchema : ResourceSchemaBase
	{
		protected virtual CrudResourceServiceBase<T, TSchema> CrudService => new CrudResourceServiceBase<T, TSchema>(Context, HttpContext, AuthService);
		protected abstract AuthorizationService<T, TSchema> AuthService { get; }

		[Route("create")]
		public IActionResult Create()
		{
			if(Context.User == null)
			{
				// redirect to login
				return Forbid();
			}
			var tokens = Context.User.TokenCollection.GetAllTokens<ResourceCreationToken>(Context, EPermissionLevel.OWNER)
				.Where(t => t.ResourceType == typeof(T).Name && !t.IsRedeemed);
			if(!tokens.Any())
			{
				return Unauthorized("You must request permission to create this resource.");
			}
			return View();
		}

		[HttpPost]
		[Route("create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([FromForm] TSchema schema)
		{
			try
			{
				if (!ModelState.IsValid) 
				{ 
					return View(schema); 
				}
				var request = await CrudService.Create(schema);
				if(!request.IsSuccess)
				{
					return request.ActionResult;
				}
				return RedirectToAction(nameof(Edit));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(schema);
			}
		}

		[Route("edit/{id}")]
		public async Task<IActionResult> Edit([FromRoute] string id)
		{
			var result = await CrudService.Get(id);
			if(!result.IsSuccess)
			{
				return result.ActionResult;
			}
			var getResult = result as ResourceActionRequest;
			ViewData["original"] = getResult.Result;
			return View(getResult.Result);
		}

		// POST: LocalGroups/Edit/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[HttpPost]
		[Route("edit/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit([FromRoute] string id, [FromForm] T modified)
		{
			try
			{
				if (!ModelState.IsValid) 
				{ 
					return View(modified); 
				}

				var original = ViewData["original"];
				//var patch = JsonViewUtility.GeneratePatch(modified, original);
				//var result = (await CrudService.Update(id, patch));

				return RedirectToAction(nameof(Edit));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(modified);
			}
		}

		// GET: LocalGroups/Delete/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[Route("delete/{id}")]
		public async Task<IActionResult> Delete([FromRoute] string id)
		{
			var result = (await CrudService.Get(id.ToString()));
			if(!result.IsSuccess)
			{
				return result.ActionResult;
			}
			var modReq = result as ResourceActionRequest;
			return View(modReq.Result);
		}

		// POST: LocalGroups/Delete/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[HttpPost]
		[Route("delete/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete([FromRoute] string id, [FromForm] T rsc)
		{
			try
			{
				var request = (await CrudService.Delete(id.ToString()));
				return RedirectToAction(nameof(Index));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(rsc);
			}
		}
	}
}
