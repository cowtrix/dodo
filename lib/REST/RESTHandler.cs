using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;

namespace REST
{
	public abstract class RESTHandler
	{
		public abstract void AddRoutes(List<Route> routeList);

		public static Func<HttpRequest, IActionResult> WrapRawCall(Func<HttpRequest, IActionResult> call)
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
					var msg = e.Message;
					if (e is HttpException)
					{
						return HttpBuilder.Custom(msg, (e as HttpException).ErrorCode);
					}
					return HttpBuilder.ServerError(msg);
				}
			};
		}
	}
}
