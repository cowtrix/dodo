using Common;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XR.Dodo
{
	public abstract class RESTHandler
	{
		public void AddRoutes(List<Route> routeList)
		{
			var allMethods = GetType().GetMethods();
			foreach(var method in allMethods)
			{
				var attr = method.GetCustomAttribute<RouteAttribute>();
				if(attr == null)
				{
					continue;
				}
				if(method.ReturnType != typeof(HttpResponse))
				{
					throw new Exception($"Invalid return type on method {GetType().Name}::{method.Name}. Expected HTTPResponse, got {method.ReturnType.Name}");
				}
				var parameters = method.GetParameters();
				if(parameters.Count() != 1 || parameters[0].ParameterType != typeof(HttpRequest))
				{
					throw new Exception($"Invalid method signature {GetType().Name}::{method.Name}. Expected single HTTPRequest parameter.");
				}
				routeList.Add(new Route()
				{
					Name = attr.Name,
					UrlRegex = attr.URLRegex,
					Method = attr.RequestType.ToString(),
					Callable = (req) =>
					{
						try
						{
							return method.Invoke(this, new[] { req }) as HttpResponse;
						}
						catch(Exception e)
						{
							Logger.Exception(e);
							return HttpBuilder.Error("Error processing request: " + e.Message);
						}
					},
				});
			}
		}
	}
}
