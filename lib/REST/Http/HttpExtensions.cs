using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace REST
{
	public static class HttpExtensions
	{
		public static string ReadBody(this HttpRequest request)
		{
			var body = new StreamReader(request.Body);
			//The modelbinder has already read the stream and need to reset the stream index
			body.BaseStream.Seek(0, SeekOrigin.Begin);
			return body.ReadToEnd();
		}

		public static EHTTPRequestType MethodEnum (this HttpRequest request)
		{
			return (EHTTPRequestType)Enum.Parse(typeof(EHTTPRequestType), request.Method);
		}

		public static Func<T, IActionResult> WrapCall<T>(Func<T, IActionResult> call)
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
					if (e.InnerException != null)
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
