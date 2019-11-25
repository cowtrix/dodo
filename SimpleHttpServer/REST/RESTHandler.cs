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
					if (e is HttpException)
					{
						return HttpBuilder.Error("Error processing request:\n" + e.Message, (e as HttpException).ErrorCode);
					}
					return HttpBuilder.Error("Error processing request:\n" + e.Message);
				}
			};
		}
	}
}
