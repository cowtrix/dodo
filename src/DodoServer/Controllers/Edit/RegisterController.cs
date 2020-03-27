using Dodo.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;
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
		public ActionResult Index()
		{
			return View();
		}

		// POST: Login/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(UserSchema model)
		{
			try
			{
				var cookieContainer = new CookieContainer();
				var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
				var client = new HttpClient(handler);
				var registerResponse = await client.PostAsJsonAsync($"{DodoServer.HttpsUrl}/{RootURL}/{REGISTER}", model);
				if (!registerResponse.IsSuccessStatusCode)
				{
					return new HttpStatusContentResult(registerResponse.StatusCode, await registerResponse.Content.ReadAsStringAsync());
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
