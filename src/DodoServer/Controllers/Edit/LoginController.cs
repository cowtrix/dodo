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
	public class LoginController : Controller
	{

		// GET: Login
		public ActionResult Index()
		{
			return View();
		}

		// POST: Login/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(LoginModel model)
		{
			try
			{
				var cookieContainer = new CookieContainer();
				var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
				var client = new HttpClient(handler);
				var loginResponse = await client.PostAsJsonAsync($"{DodoServer.HttpsUrl}/{RootURL}/{LOGIN}", model);
				var cookies = cookieContainer.GetCookies(new Uri(DodoServer.HttpsUrl));
				foreach (Cookie cookie in cookies)
				{
					Response.Cookies.Append(cookie.Name, cookie.Value, new CookieOptions
					{
						HttpOnly = true,
						Secure = true,
						SameSite = SameSiteMode.Strict,
					});
				}

				return RedirectToAction(nameof(Index), "Rebellions");
			}
			catch
			{
				return View();
			}
		}

	}
}
