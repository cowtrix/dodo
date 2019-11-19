using Common;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SimpleHttpServer.REST
{
	public abstract class RESTHandler
	{
		public virtual void AddRoutes(List<Route> routeList)
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
				routeList.Add(new Route(attr.Name, attr.RequestType, (url) => Regex.Match(url, attr.URLRegex).Success,
					WrapRawCall((req) => method.Invoke(this, new[] { req }) as HttpResponse)));
				Logger.Debug($"Added route for {attr.Name} @ {attr.URLRegex}");
			}
		}

		protected Func<HttpRequest, HttpResponse> WrapRawCall(Func<HttpRequest, HttpResponse> call)
		{
			return (req) =>
			{
				try
				{
					return call(req);
				}
				catch (Exception e)
				{
					Logger.Exception(e);
					if(e.InnerException != null)
					{
						e = e.InnerException;
					}
					if (e is HTTPException)
					{
						return HttpBuilder.Error("Error processing request:\n" + e.Message, (e as HTTPException).ErrorCode);
					}
					return HttpBuilder.Error("Error processing request:\n" + e.Message);
				}
			};
		}
	}
}
