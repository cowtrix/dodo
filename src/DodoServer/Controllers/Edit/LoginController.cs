using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static Dodo.Users.UserController;

namespace DodoServer.Controllers.Edit
{
	[AllowAnonymous]
	[Route(LOGIN)]
	public class LoginController : CrudController
	{
		// GET: Login
		public ActionResult Index(string redirect = null)
		{
			// Controller life-cycle is per request so we need to give this to the client to get it back
			ViewData["redirect"] = redirect;
			return View();
		}

		// POST: Login/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(LoginModel model)
		{
			ViewData["redirect"] = model.redirect;
			try
			{
				var cookieContainer = new CookieContainer();
				var handler = new HttpClientHandler()
				{
					CookieContainer = cookieContainer,
				};
				// This needs to go to the login endpoint, which doesn't currently sit under /API
				var client = GetHttpClient(RootURL, cookieContainer, false);
				var loginResponse = await client.PostAsJsonAsync(LOGIN, model);
				if (!loginResponse.IsSuccessStatusCode)
				{
					// Put error on model state
					ModelState.AddModelError("", "Log in failed (wrong username / password?)");
					return View(model);
				}
				var cookies = cookieContainer.GetCookies(new Uri(DodoServer.NetConfig.FullURI));
				foreach (Cookie cookie in cookies)
				{
					Response.Cookies.Append(cookie.Name, cookie.Value, new CookieOptions
					{
						HttpOnly = true,
						Secure = true,
						SameSite = SameSiteMode.Strict,
					});
				}

				// Guard against open redirect attack
				if (Url.IsLocalUrl(model.redirect))
				{
					return Redirect(model.redirect);
				}
				return Redirect(DodoServer.NetConfig.FullURI);
			}
			catch
			{
				return View(model);
			}
		}

	}
}
