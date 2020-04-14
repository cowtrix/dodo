using System;
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
	public class RebellionsController : CrudController
	{
		// GET: Rebellions
		[AllowAnonymous]
		public async Task<IActionResult> Index()
		{
			return await GetResourcesView<Rebellion>($"{DodoServer.API_ROOT}{RebellionController.RootURL}");
		}

		// GET: Rebellions/Details/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[AllowAnonymous]
		public async Task<IActionResult> Details(Guid id)
		{
			return await GetResourceView<Rebellion>($"{DodoServer.API_ROOT}{RebellionController.RootURL}", id);
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
				var client = GetHttpClient($"{DodoServer.API_ROOT}{RebellionController.RootURL}");

				var result = await client.PostAsJsonAsync("", rebellion);
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
			return await GetResourceView<Rebellion>($"{DodoServer.API_ROOT}{RebellionController.RootURL}", id);
		}

		// POST: Rebellions/Edit/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(Guid id, Rebellion rebellion)
		{
			try
			{
				if (!ModelState.IsValid) return View(rebellion);
				var client = GetHttpClient($"{DodoServer.API_ROOT}{RebellionController.RootURL}");

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

				var result = await client.PatchAsync($"{id}", content);
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
			return await GetResourceView<Rebellion>($"{DodoServer.API_ROOT}{RebellionController.RootURL}", id);
		}

		// POST: Rebellions/Delete/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(Guid id, Rebellion rebellion)
		{
			try
			{
				var client = GetHttpClient($"{DodoServer.API_ROOT}{RebellionController.RootURL}");

				var result = await client.DeleteAsync($"{id}");
				result.EnsureSuccessStatusCode();

				return RedirectToAction(nameof(Index));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return await GetResourceView<Rebellion>($"{DodoServer.API_ROOT}{RebellionController.RootURL}", id);
			}
		}

	}
}
