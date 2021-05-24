using System;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Dodo.Rebellions;
using Dodo.LocalGroups;
using Dodo.WorkingGroups;
using Dodo.LocationResources;
using Dodo.Roles;
using Resources;

namespace Dodo
{
	[Route(DodoApp.API_ROOT)]
	[AllowAnonymous]
	public class APIController : Controller
	{
		public static string GetDisplayColor(Type t)
		{
			if (t == null || !TypeDisplayColors.TryGetValue(t, out var c))
			{
				return "FFFFF";
			}
			return c;
		}

		private static Dictionary<Type, string> TypeDisplayColors = new Dictionary<Type, string>()
		{
			{ typeof(Rebellion),		"22A73D" },
			{ typeof(LocalGroup),		"71D0F1" },
			{ typeof(WorkingGroup),		"FFC113" },
			{ typeof(Event),			"ED9BC4" },
			{ typeof(Site),				"22A73D" },
			{ typeof(Role),				"E53D33" },
		};

		static APIController()
		{
			MetadataObject = new
			{
				resourceTypes = ReflectionExtensions.GetConcreteClasses<IDodoResource>()
					.Where(t => typeof(IPublicResource).IsAssignableFrom(t)
						&& t != typeof(Role) && t != typeof(WorkingGroup))
					.Select(t => new
					{
						label = t.GetName(),
						value = t.Name.ToCamelCase(),
						displayColor = TypeDisplayColors[t],
						schema = JsonViewUtility.GetJsonSchema(t)
					}).ToList(),
				privacyPolicy = DodoApp.PrivacyPolicyURL,
				rebelAgreement = DodoApp.RebelAgreementURL,
			};
		}

		static dynamic MetadataObject { get; set; }

		[HttpGet]
		public IActionResult Get()
		{
			return Content(JsonConvert.SerializeObject(MetadataObject, JsonExtensions.NetworkSettings),
				"application/json; charset=utf-8");
		}
	}
}
