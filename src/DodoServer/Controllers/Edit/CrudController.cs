using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DodoServer.Controllers.Edit
{
	[Authorize]
	public class CrudController : Controller
	{
		// DodoURI_Https in DodoServer_config.json must be set to actual IP or localhost (not 0.0.0.0)
		private static readonly string baseApiUrl = $"{DodoServer.HttpsUrl}/{DodoServer.API_ROOT}";

		internal async Task<IActionResult> GetResourcesView<T>(string resourceUrl)
		{
			var client = new HttpClient();
			var httpResponse = await client.GetAsync($"{baseApiUrl}{resourceUrl}");
			httpResponse.EnsureSuccessStatusCode();
			var resources = await httpResponse.Content.ReadAsAsync<IEnumerable<T>>();
			return View(resources);
		}

		internal async Task<IActionResult> GetResourceView<T>(string resourceUrl, Guid id)
		{
			var client = new HttpClient();
			var httpResponse = await client.GetAsync($"{baseApiUrl}{resourceUrl}/{id}");
			httpResponse.EnsureSuccessStatusCode();
			var resource = await httpResponse.Content.ReadAsAsync<T>();
			return View(resource);
		}

		internal HttpClient GetHttpClient(string resourceUrl)
		{
			// Best practice for HttpClient is to use a singleton and not dispose of it in using statement
			// However, as these are authenticated requests with cookies it is safer to use a new instance
			// TODO: Keep an eye on this under load to see if it causes perf problems or socket exhaustion
			var cookieContainer = new CookieContainer();
			var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
			var client = new HttpClient(handler);
			client.BaseAddress = new Uri($"{baseApiUrl}{resourceUrl}/");
			foreach (var cookie in Request.Cookies)
			{
				cookieContainer.Add(new Cookie(cookie.Key, cookie.Value, "/", "localhost"));
			}
			return client;
		}
	}
}