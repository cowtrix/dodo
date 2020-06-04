using System;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using Common;
using Dodo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Dodo.Users;
using Dodo.Rebellions;
using Dodo.LocalGroups;
using Dodo.WorkingGroups;
using Dodo.Sites;
using Dodo.Roles;
using Common.Config;

namespace DodoServer
{
	[Route(DodoServer.API_ROOT)]
	[AllowAnonymous]
	public class APIController : Controller
	{
		static Dictionary<Type, string> m_displayColors = new Dictionary<Type, string>()
		{
			{ typeof(Rebellion),		"22A73D" },
			{ typeof(LocalGroup),		"71D0F1" },
			{ typeof(WorkingGroup),		"FFC113" },
			{ typeof(EventSite),		"ED9BC4" },
			{ typeof(MarchSite),		"ED9BC4" },
			{ typeof(PermanentSite),	"22A73D" },
			{ typeof(Role),				"E53D33" },
		};

		static APIController()
		{
			MetadataObject = new
			{
				resourceTypes = ReflectionExtensions.GetConcreteClasses<IDodoResource>()
					.Where(t => t != typeof(User))
					.Select(t => new { label = t.GetName(), value = t.Name.ToCamelCase(), displayColor = m_displayColors[t] }).ToList(),
				indexVideoEmbed = ConfigManager.GetValue("IndexVideoEmbedURL", "https://www.youtube.com/embed/d4QDM_Isi24"),
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