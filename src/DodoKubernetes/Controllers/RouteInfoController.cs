using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Net;
using System.Text;

namespace REST
{
	[Route("routes")]
	public class RouteInfoController : Controller
	{
		// for accessing conventional routes...
		private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

		public RouteInfoController(
			IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
		{
			_actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
		}

		public IActionResult Index()
		{
			StringBuilder sb = new StringBuilder();

			foreach (ActionDescriptor ad in _actionDescriptorCollectionProvider.ActionDescriptors.Items)
			{
				var action = Url.Action(new UrlActionContext()
				{
					Action = ad.RouteValues["action"],
					Controller = ad.RouteValues["controller"],
					Values = ad.RouteValues
				});
				sb.AppendLine($"{ad.DisplayName}:\t{ad.AttributeRouteInfo.Template}\t{action}").AppendLine();
			}

			return Ok(sb.ToString());
		}
	}
}
