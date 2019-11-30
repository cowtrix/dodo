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
		public abstract void AddRoutes(List<Route> routeList);

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
					var msg = Uri.EscapeDataString(e.Message);
					if (e is HttpException)
					{
						return HttpBuilder.Custom("Error processing request:\n" + msg, (e as HttpException).ErrorCode);
					}
					return HttpBuilder.ServerError("Error processing request:\n" + msg);
				}
			};
		}
	}
}
