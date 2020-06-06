using Common;
using Dodo.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dodo.Controllers.Edit
{
	[Route("register")]
	public class RegisterController : Controller
	{
		// GET: Login
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
				var userController = new UserController();
				var result = await userController.Register(model, token);
				if(!(result is OkResult))
				{
					//ModelState.AddModelError(result.GetType(), $"{result.}");
					return View(result);
				}
				// Guard against open redirect attack
				if (Url.IsLocalUrl(redirect))
				{
					// As redirect is user provided we should not trust it
					// Ensure redirect is URL encoded when used as a query string parameter
					return base.Redirect(QueryHelpers.AddQueryString($"{(Dodo.NetConfig.FullURI)}/{UserController.LOGIN}", "redirect", redirect));
				}
				return Redirect($"{Dodo.NetConfig.FullURI}/{UserController.LOGIN}");
			}
			catch
			{
				return View();
			}
		}
	}
}
