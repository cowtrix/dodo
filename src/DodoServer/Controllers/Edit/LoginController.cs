using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resources;
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
		string m_redirect;
		// GET: Login
		public ActionResult Index(string redirect = null)
		{
			m_redirect = redirect;
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
				if(!loginResponse.IsSuccessStatusCode)
				{
					return new HttpStatusContentResult(loginResponse.StatusCode, await loginResponse.Content.ReadAsStringAsync());
				}
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

				return Redirect(model.redirect ?? m_redirect ?? DodoServer.Homepage);
			}
			catch
			{
				return View();
			}
		}
	}
}
