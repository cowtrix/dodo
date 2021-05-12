using Common.Commands;
using Dodo;
using Dodo.LocationResources;
using Dodo.RoleApplications;
using Dodo.Users;
using Dodo.Users.Tokens;
using Dodo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;
using Resources.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dodo.Sites
{
	public class SysadminViewModel
	{
		public string Command { get; set; }
	}

	[Route(RootURL)]
	public class SysadminController : CustomController
	{
		public const string RootURL = "admin";
		public const string COMMAND_HISTORY = "history";
		public const string CLEAR = "clear";

		[HttpGet]
		public virtual async Task<IActionResult> ViewAdmin()
		{
			if (Context.User == null)
			{
				// redirect to login
				return Forbid();
			}
			var token = Context.User.TokenCollection.GetSingleToken<SysadminToken>(Context, EPermissionLevel.SYSTEM, Context.User);
			if (token == null)
			{
				return Unauthorized();
			}
			ViewData[COMMAND_HISTORY] = token.CommandHistory ?? new Queue<string>();
			var model = new SysadminViewModel();
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public virtual async Task<IActionResult> Command([FromForm] SysadminViewModel modified)
		{
			if (Context.User == null)
			{
				// redirect to login
				return Forbid();
			}
			var token = Context.User.TokenCollection.GetSingleToken<SysadminToken>(Context, EPermissionLevel.SYSTEM, Context.User);
			if (token == null)
			{
				return Unauthorized();
			}
			try
			{
				if (!ModelState.IsValid)
				{
					var errors = ModelState.Values.Where(v => v.Errors.Any()).ToList();
					ModelState.AddModelError("Save Error", "Error updating the resource");
					return await ViewAdmin();
				}
				if(!string.IsNullOrEmpty(modified.Command))
				{
					var output = CommandManager.Execute(modified.Command);

					using var rscLock = new ResourceLock(Context.User);
					var user = rscLock.Value as User;
					token = user.TokenCollection.GetSingleToken<SysadminToken>(Context, EPermissionLevel.SYSTEM, Context.User);
					if (token == null)
					{
						return Unauthorized();
					}
					if (token.CommandHistory == null)
					{
						token.CommandHistory = new Queue<string>();
					}
					token.CommandHistory.Enqueue(modified.Command.Trim());
					token.CommandHistory.Enqueue(output.Trim());
					user.TokenCollection.AddOrUpdate(user, token);
					ResourceUtility.GetManager<User>().Update(user, rscLock);
				}
				return RedirectToAction(nameof(ViewAdmin));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return await ViewAdmin();
			}
		}

		[HttpGet]
		[Route(CLEAR)]
		public virtual async Task<IActionResult> Clear()
		{
			if (Context.User == null)
			{
				// redirect to login
				return Forbid();
			}
			var token = Context.User.TokenCollection.GetSingleToken<SysadminToken>(Context, EPermissionLevel.SYSTEM, Context.User);
			if (token == null)
			{
				return Unauthorized();
			}
			try
			{
				using var rscLock = new ResourceLock(Context.User);
				var user = rscLock.Value as User;
				token = user.TokenCollection.GetSingleToken<SysadminToken>(Context, EPermissionLevel.SYSTEM, Context.User);
				if (token == null)
				{
					return Unauthorized();
				}
				if (token.CommandHistory == null)
				{
					token.CommandHistory = new Queue<string>();
				}
				token.CommandHistory.Clear();
				user.TokenCollection.AddOrUpdate(user, token);
				ResourceUtility.GetManager<User>().Update(user, rscLock);
				return RedirectToAction(nameof(ViewAdmin));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return await ViewAdmin();
			}
		}
	}
}
