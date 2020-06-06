using Common;
using Dodo.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Resources;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dodo.Controllers.Edit
{
	[Route(UserService.REGISTER)]
	public class RegisterController : CustomController
	{
		protected UserService UserService => new UserService(Context, HttpContext);

		[HttpGet]
		public ActionResult Index(string token = null, string redirect = null)
		{
			ViewData["redirect"] = redirect;
			ViewData["token"] = token;
			return View();
		}

		// POST: Login/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(UserSchema model, string token = null, string redirect = null)
		{
			ViewData["redirect"] = redirect;
			ViewData["token"] = token;
			try
			{
				var result = await UserService.Register(model, token);
				if (!result.IsSuccess)
				{
					//ModelState.AddModelError(result.GetType(), $"{result.}");
					return result.Result;
				}
				// Guard against open redirect attack
				if (Url.IsLocalUrl(redirect))
				{
					// As redirect is user provided we should not trust it
					// Ensure redirect is URL encoded when used as a query string parameter
					return base.Redirect(QueryHelpers.AddQueryString($"{(Dodo.NetConfig.FullURI)}/{UserService.LOGIN}", "redirect", redirect));
				}
				return Redirect($"{Dodo.NetConfig.FullURI}/{UserService.LOGIN}");
			}
			catch
			{
				return View();
			}
		}
	}

}
