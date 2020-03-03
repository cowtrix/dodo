using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Resources
{
	[Route("api")]
	public class RouteInfoController : Controller
	{
		private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
#if DEBUG
		static string TableStyle => System.IO.File.ReadAllText(Path.Combine(@"..\..\resources", "EmbeddedCss.css"));
#else
		static string TableStyle => System.IO.File.ReadAllText(Path.Combine("resources", "EmbeddedCss.css"));
#endif

		List<Type> m_documentedTypes = new List<Type>()
		{
			typeof(ResourceSchemaBase)
		};

		public RouteInfoController(
			IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
		{
			_actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
		}

		public IActionResult Index()
		{
			List<List<string>> table = new List<List<string>>();
			table.Add(new List<string>()
			{
				"Name",
				"URI",
				//"Action",
				"Method(s)",
				"Parameters"
			});
			foreach (ActionDescriptor ad in _actionDescriptorCollectionProvider.ActionDescriptors.Items)
			{
				var controllerDescriptor = ad as ControllerActionDescriptor;
				var action = Url.Action(new UrlActionContext()
				{
					Action = ad.RouteValues["action"],
					Controller = ad.RouteValues["controller"],
					Values = ad.RouteValues
				});

				table.Add(new List<string>()
				{
					StripFriendlyName(ad?.DisplayName),
					ad?.AttributeRouteInfo?.Template,
					//action,
					string.Join(", ", ad?.ActionConstraints?.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods ?? new [] { "" }),
					FormatAPIParams(controllerDescriptor?.MethodInfo?.GetParameters()),
				});
			}
			return Content(CollectionToHtmlTable(table), new MediaTypeHeaderValue("text/html"));
		}

		string FormatAPIParams(IEnumerable<ParameterInfo> parameters)
		{
			var sb = new StringBuilder();
			foreach (var p in parameters)
			{
				bool isDocumentedType = m_documentedTypes.Any(t => t.IsAssignableFrom(p.ParameterType));
				var prettyParamTypeName = GetPrettyTypeName(p.ParameterType);
				if (isDocumentedType)
				{
					sb.Append($"<a href=\"#{p.ParameterType.Name}\">");
				}
				sb.AppendLine($"{p.Name}:{p.ParameterType}");
				if (p.GetCustomAttribute<FromBodyAttribute>() != null)
				{
					sb.Append(" [JSON Body]");
				}
				if (isDocumentedType)
				{
					sb.Append("</a>");
				}
				sb.Append("</br>");
			}
			return sb.ToString();
		}

		string CollectionToHtmlTable(IEnumerable<IEnumerable<string>> table)
		{
			var sb = new StringBuilder("<!DOCTYPE html><html><body><table>");
			sb.Append($"<style>{TableStyle}</style>");

			sb.Append("<thead><tr><th>");
			sb.Append(string.Join("</th><th>", table.First()));
			sb.AppendLine("</th></tr></thead><tbody>");

			foreach (var row in table.Skip(1))
			{
				sb.Append("<tr><td>");
				sb.Append(string.Join("</td><td>", row));
				sb.AppendLine("</td></tr>");
			}
			sb.Append("</tbody></table></html></body>");
			return sb.ToString();
		}

		private string StripFriendlyName(string name)
		{
			var lastDot = name.IndexOf('.');
			if (lastDot > 0)
			{
				name = name.Substring(lastDot + 1);
			}
			var firstBracket = name.IndexOf('(');
			if (firstBracket > 0)
			{
				name = name.Substring(0, firstBracket - 1);
			}
			return name;
		}

		static string GetPrettyTypeName(Type t)
		{
			var tArgs = t.IsGenericType ? t.GetGenericArguments() : Type.EmptyTypes;
			var name = t.Name;
			var format = Regex.Replace(name, @"`\d+.*", "") + (t.IsGenericType ? "<?>" : "");
			var names = tArgs.Select(x => x.IsGenericParameter ? "" : GetPrettyTypeName(x));
			return String.Join(String.Join(",", names), format.Split('?'));
		}
	}
}
