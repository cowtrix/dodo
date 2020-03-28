using Dodo.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static Dodo.Users.UserController;

namespace DodoServer.Controllers.Edit
{
	[AllowAnonymous]
	[Route(REGISTER)]
	public class RegisterController : Controller
	{
		// GET: Login
		public ActionResult Index(string redirect = null)
		{
			ViewData["redirect"] = redirect;
			return View();
		}

		// POST: Login/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(UserSchema model, string redirect = null)
		{
			ViewData["redirect"] = redirect;
			try
			{
				var cookieContainer = new CookieContainer();
				var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
				var client = new HttpClient(handler);
				var registerResponse = await client.PostAsJsonAsync($"{DodoServer.HttpsUrl}/{RootURL}/{REGISTER}", model);
				if (!registerResponse.IsSuccessStatusCode)
				{
					ModelState.AddModelError("", "Register failed");
					return View(model);
				}

				// Guard against open redirect attack
				if (Url.IsLocalUrl(redirect))
				{
					// As redirect is user provided we should not trust it
					// Ensure redirect is URL encoded when used as a query string parameter
					return Redirect(QueryHelpers.AddQueryString($"{DodoServer.HttpsUrl}/{LOGIN}", "redirect", redirect));
				}
				return Redirect($"{DodoServer.HttpsUrl}/{LOGIN}");

			}
			catch
			{
				return View();
			}
		}
	}
}
