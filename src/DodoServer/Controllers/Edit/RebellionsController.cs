using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dodo.Rebellions;
using DodoResources.Rebellions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Resources;

namespace DodoServer.Controllers.Edit
{
	// TODO: Auth
	public class RebellionsController : Controller
	{
		private IResourceManager<Rebellion> ResourceManager => ResourceUtility.GetManager<Rebellion>();

		// GET: Rebellions
		public IActionResult Index()
		{
			return View(ResourceManager.Get(r => true));
		}

		// GET: Rebellions/Details/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		public IActionResult Details(Guid id)
		{
			return View(ResourceManager.GetSingle(r => r.GUID == id));
		}

		// GET: Rebellions/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Rebellions/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(IFormCollection rebellion)
		{
			try
			{
				// TODO: Graceful value parsing
				var location = new GeoLocation(double.Parse(rebellion["Location.Latitude"]), double.Parse(rebellion["Location.Longitude"]));
				var schema = new RebellionSchema(rebellion["Name"], rebellion["PublicDescription"], location, DateTime.Parse(rebellion["StartDate"]), DateTime.Parse(rebellion["EndDate"]));

				// TODO: Create direct or call API controller via HTTP? (can't call method on controller directly as Request there will be null)

				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: Rebellions/Edit/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		public IActionResult Edit(Guid id)
		{
			return View(ResourceManager.GetSingle(r => r.GUID == id));
		}

		// POST: Rebellions/Edit/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Guid id, IFormCollection rebellion)
		{
			try
			{
				// TODO: Add update logic here

				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		// GET: Rebellions/Delete/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		public IActionResult Delete(Guid id)
		{
			return View(ResourceManager.GetSingle(r => r.GUID == id));
		}

		// POST: Rebellions/Delete/0a985dee-0b68-4805-96f5-3abe6f1ae13e
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Delete(Guid id, IFormCollection rebellion)
		{
			try
			{
				// TODO: Add delete logic here

				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
	}
}
