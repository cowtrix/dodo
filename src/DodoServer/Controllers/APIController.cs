using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;
using Common;
using Dodo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Dodo.Users;

namespace DodoServer
{
	[Route(DodoServer.API_ROOT)]
	[AllowAnonymous]
	public class APIController : Controller
	{
		static APIController()
		{
			MetadataObject = new
			{
				resourceTypes = ReflectionExtensions.GetConcreteClasses<IDodoResource>()
					.Where(t => t != typeof(User))
					.Select(t => new { label = t.GetName(), value = t.Name.ToCamelCase() }).ToList(),
			};
		}

		static dynamic MetadataObject { get; set; }

		[HttpGet]
		public IActionResult Get()
		{
			return Content(JsonConvert.SerializeObject(MetadataObject, JsonExtensions.NetworkSettings));
		}
	}
}
