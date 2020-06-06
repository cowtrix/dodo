using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Dodo;
using Dodo.Users.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;

namespace Dodo.Controllers.Edit
{
	[Route("edit/[controller]")]
	public abstract class CrudController<T, TSchema> : CustomController
		where T : DodoResource, IPublicResource
		where TSchema : ResourceSchemaBase
	{
		protected abstract PublicResourceAPIController<T, TSchema> RESTController { get; }

		// GET: Rebellions/Create
		[Route("create")]
		public IActionResult Create()
		{
			if(Context.User == null)
			{
				// redirect to login
				return Forbid();
			}
			var tokens = Context.User.TokenCollection.GetAllTokens<ResourceCreationToken>(Context)
				.Where(t => t.ResourceType == typeof(T).Name && !t.IsRedeemed);
			if(!tokens.Any())
			{
				return Unauthorized("You must request permission to create this resource.");
			}
			return View();
		}

		// POST: Rebellions/Create
		[HttpPost]
		[Route("create")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([FromForm] T rsc)
		{
			try
			{
				if (!ModelState.IsValid) 
				{ 
					return View(rsc); 
				}
				return View(rsc);
			}
			catch (Exception e)
			{
				ModelState.AddModelError("", e.Message);
				return View(rsc);
			}
		}
	}
}
