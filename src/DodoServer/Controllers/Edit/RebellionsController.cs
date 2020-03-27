using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DodoResources.Rebellions;
using DodoServer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DodoServer.Controllers.Edit
{
	[Authorize]
	public class RebellionsController : Controller
	{
		// DodoURI_Https in DodoServer_config.json must be set to actual IP or localhost (not 0.0.0.0) for this to work

		// GET: Rebellions
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			var client = new HttpClient();
			var httpResponse = await client.GetAsync($"{DodoServer.HttpsUrl}/api/rebellions");
			httpResponse.EnsureSuccessStatusCode();
			var rebellions = await httpResponse.Content.ReadAsAsync<IEnumerable<Rebellion>>();
			return View(rebellions);
		}

		// GET: Rebellions/Details/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[AllowAnonymous]
		public async Task<IActionResult> Details(Guid id)
		{
			var client = new HttpClient();
			var httpResponse = await client.GetAsync($"{DodoServer.HttpsUrl}/api/rebellions/{id}");
			httpResponse.EnsureSuccessStatusCode();
			var rebellion = await httpResponse.Content.ReadAsAsync<Rebellion>();
			return View(rebellion);
		}

		// GET: Rebellions/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Rebellions/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Rebellion rebellion)
		{
			try
			{
				if (!ModelState.IsValid) return View(rebellion);
				var client = GetHttpClient();

				// Currently you can't create a rebellion as the user needs to have admin on the parent
				// Rebellions (and Local Groups) don't have a parent as they are top level entities
				// This works if the parent check is bypassed in GroupResourceAuthManager.CanCreate
				// The actual method to authorize a user for rebellion creation is to be decided
				var result = await client.PostAsJsonAsync($"{DodoServer.HttpsUrl}/api/rebellions", rebellion);
				result.EnsureSuccessStatusCode();

				return RedirectToAction(nameof(Index));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(rebellion);
			}
		}

		// GET: Rebellions/Edit/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		public async Task<IActionResult> Edit(Guid id)
		{
			var client = new HttpClient();
			var httpResponse = await client.GetAsync($"{DodoServer.HttpsUrl}/api/rebellions/{id}");
			httpResponse.EnsureSuccessStatusCode();
			var rebellion = await httpResponse.Content.ReadAsAsync<Rebellion>();
			return View(rebellion);
		}

		// POST: Rebellions/Edit/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, Rebellion rebellion)
		{
			try
			{
				if (!ModelState.IsValid) return View(rebellion);
				var client = GetHttpClient();

				// Can't use Rebellion View Model as read-only properties causes security exceptions
				// Can't use RebellionSchema as Parent gets serialized and that causes issues
				// If using JSON.NET then GeoLocation.m_reverseGeocodingKey is also serialized
				var dto = new RebellionDto
				{
					Name = rebellion.Name,
					PublicDescription = rebellion.PublicDescription,
					Location = rebellion.Location,
					StartDate = rebellion.StartDate,
					EndDate = rebellion.EndDate,
				};
				var json = System.Text.Json.JsonSerializer.Serialize(dto);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				// This currently throws a resource locked exception for rebellions with working groups
				var result = await client.PatchAsync($"{DodoServer.HttpsUrl}/api/rebellions/{id}", content);
				result.EnsureSuccessStatusCode();

				return RedirectToAction(nameof(Index));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(rebellion);
			}
		}

		// GET: Rebellions/Delete/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		public async Task<IActionResult> Delete(Guid id)
		{
			var client = new HttpClient();
			var httpResponse = await client.GetAsync($"{DodoServer.HttpsUrl}/api/rebellions/{id}");
			httpResponse.EnsureSuccessStatusCode();
			var rebellion = await httpResponse.Content.ReadAsAsync<Rebellion>();
			return View(rebellion);
		}

		// POST: Rebellions/Delete/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(Guid id, Rebellion rebellion)
		{
			try
			{
				var client = GetHttpClient();

				var result = await client.DeleteAsync($"{DodoServer.HttpsUrl}/api/rebellions/{id}");
				result.EnsureSuccessStatusCode();

				return RedirectToAction(nameof(Index));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(rebellion);
			}
		}

		private HttpClient GetHttpClient()
		{
			// Best practice for HttpClient is to use a singleton and not dispose of it in using statement
			// However, as these are authenticated requests with cookies it is safer to use a new instance
			// TODO: Keep an eye on this under load to see if it causes perf problems or socket exhaustion
			var cookieContainer = new CookieContainer();
			var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
			var client = new HttpClient(handler);

			foreach (var cookie in Request.Cookies)
			{
				cookieContainer.Add(new Cookie(cookie.Key, cookie.Value, "/", "localhost"));
			}

			return client;
		}
	}
}
