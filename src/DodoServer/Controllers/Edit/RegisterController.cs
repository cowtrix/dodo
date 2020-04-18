using Common;
using Dodo.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static Dodo.Users.UserController;

namespace DodoServer.Controllers.Edit
{
	[AllowAnonymous]
	[Route(REGISTER)]
	public class RegisterController : CrudController
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
				var client = GetHttpClient(RootURL, null, false);
				var registerResponse = await client.PostAsJsonAsync(REGISTER, model);
				if (!registerResponse.IsSuccessStatusCode)
				{
					ModelState.AddModelError("", $"Register failed: {registerResponse.ReasonPhrase}\n{await registerResponse.Content.ReadAsStringAsync()}");
					return View(model);
				}

				// Guard against open redirect attack
				if (Url.IsLocalUrl(redirect))
				{
					// As redirect is user provided we should not trust it
					// Ensure redirect is URL encoded when used as a query string parameter
					return Redirect(QueryHelpers.AddQueryString($"{DodoServer.NetConfig.FullURI}/{LOGIN}", "redirect", redirect));
				}
				return Redirect($"{DodoServer.NetConfig.FullURI}/{LOGIN}");

			}
			catch
			{
				return View();
			}
		}
	}
}
