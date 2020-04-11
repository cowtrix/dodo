using Common;
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
	public class LoginController : Controller
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
#if DEBUG
					// TODO: REMOVE DangerousAcceptAnyServerCertificateValidator AS SOON AS IS REASONABLE!
					ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
#endif
				};
				var client = new HttpClient(handler);
				if (HttpClientHandler.DangerousAcceptAnyServerCertificateValidator ==
					handler.ServerCertificateCustomValidationCallback)
				{
					Logger.Warning("Server certificate is not being validated!");
				}
				var loginResponse = await client.PostAsJsonAsync($"{DodoServer.HttpsUri}/{RootURL}/{LOGIN}", model);
				if (!loginResponse.IsSuccessStatusCode)
				{
					// Put error on model state
					ModelState.AddModelError("", "Log in failed (wrong username / password?)");
					return View(model);
				}
				var cookies = cookieContainer.GetCookies(new Uri(DodoServer.HttpsUri));
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
				return Redirect(DodoServer.HttpsUri);
			}
			catch
			{
				return View(model);
			}
		}

	}
}
