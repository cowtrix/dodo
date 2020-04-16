using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DodoResources.LocalGroups;
using DodoServer.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DodoServer.Controllers.Edit
{
	[Authorize]
	public class LocalGroupsController : CrudController
	{
		// GET: LocalGroups
		[AllowAnonymous]
		[Route("")]
		public async Task<IActionResult> Index()
		{
			return await GetResourcesView<LocalGroup>(LocalGroupController.RootURL);
		}

		// GET: LocalGroups/Details/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[AllowAnonymous]
		[Route("details/{id}")]
		public async Task<IActionResult> Details([FromRoute] Guid id)
		{
			return await GetResourceView<LocalGroup>(LocalGroupController.RootURL, id);
		}

		// GET: LocalGroups/Create
		[Route("create")]
		public IActionResult Create()
		{
			return View();
		}

		// POST: LocalGroups/Create

		[HttpPost]
		[Route("create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([FromForm] LocalGroup group)
		{
			try
			{
				if (!ModelState.IsValid) return View(group);
				var client = GetHttpClient(LocalGroupController.RootURL);

				// Currently you can't create a local group as the user needs to have admin on the parent
				// Local Groups don't have a parent as they are top level entities
				// This works if the parent check is bypassed in GroupResourceAuthManager.CanCreate
				// The actual method to authorize a user for local group creation is to be decided
				var result = await client.PostAsJsonAsync("", group);
				result.EnsureSuccessStatusCode();

				return RedirectToAction(nameof(Index));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(group);
			}
		}

		// GET: LocalGroups/Edit/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[Route("edit/{id}")]
		public async Task<IActionResult> Edit([FromRoute] Guid id)
		{
			return await GetResourceView<LocalGroup>(LocalGroupController.RootURL, id);
		}

		// POST: LocalGroups/Edit/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[HttpPost]
		[Route("edit/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit([FromRoute] Guid id, [FromForm] LocalGroup group)
		{
			try
			{
				if (!ModelState.IsValid) return View(group);
				var client = GetHttpClient(LocalGroupController.RootURL);

				var dto = new LocalGroupDto
				{
					Name = group.Name,
					PublicDescription = group.PublicDescription,
					Location = group.Location,
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
				return View(group);
			}
		}

		// GET: LocalGroups/Delete/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[Route("delete/{id}")]
		public async Task<IActionResult> Delete([FromRoute] Guid id)
		{
			return await GetResourceView<LocalGroup>(LocalGroupController.RootURL, id);
		}

		// POST: LocalGroups/Delete/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[HttpPost]
		[Route("delete/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete([FromRoute] Guid id, [FromForm] LocalGroup group)
		{
			try
			{
				var client = GetHttpClient(LocalGroupController.RootURL);

				var result = await client.DeleteAsync($"{id}");
				result.EnsureSuccessStatusCode();

				return RedirectToAction(nameof(Index));
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return await GetResourceView<LocalGroup>(LocalGroupController.RootURL, id);
			}
		}
	}
}
