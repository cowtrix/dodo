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
	[Route("edit/[controller]")]
	public class CrudController : Controller
	{
		internal async Task<IActionResult> GetResourcesView<T>(string resourceUrl)
		{
			var client = GetHttpClient(resourceUrl);
			var httpResponse = await client.GetAsync(string.Empty);
			httpResponse.EnsureSuccessStatusCode();
			var resources = await httpResponse.Content.ReadAsAsync<IEnumerable<T>>();
			return View(resources);
		}

		internal async Task<IActionResult> GetResourceView<T>(string resourceUrl, Guid id)
		{
			var client = GetHttpClient(resourceUrl);
			var httpResponse = await client.GetAsync(id.ToString());
			httpResponse.EnsureSuccessStatusCode();
			var resource = await httpResponse.Content.ReadAsAsync<T>();
			return View(resource);
		}

		internal HttpClient GetHttpClient(
			string resourceUrl,
			CookieContainer cookieContainer = null,
			bool useApiRoot = true)
		{
			// Best practice for HttpClient is to use a singleton and not dispose of it in using statement
			// However, as these are authenticated requests with cookies it is safer to use a new instance
			// TODO: Keep an eye on this under load to see if it causes perf problems or socket exhaustion
			cookieContainer ??= new CookieContainer();
			var handler = new HttpClientHandler()
			{
				CookieContainer = cookieContainer,
			};
			var rootAddress = DodoServer.NetConfig.FullURI;
#if DEBUG
			if (DodoServer.NetConfig.SSLPort != 443 && !rootAddress.EndsWith(DodoServer.NetConfig.SSLPort.ToString()))
			{
				rootAddress += $":{DodoServer.NetConfig.SSLPort}";
			}
#endif
			rootAddress += useApiRoot ? $"/{DodoServer.API_ROOT}" : "/";
			var client = new HttpClient(handler)
			{
				BaseAddress = new Uri($"{rootAddress}{resourceUrl}/"),
			};
			foreach (var cookie in Request.Cookies)
			{
				cookieContainer.Add(new Cookie(cookie.Key, cookie.Value, "/", DodoServer.NetConfig.Domain));
			}
			return client;
		}
	}
}
