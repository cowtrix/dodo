using Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace REST
{
	public static class HttpExtensions
	{
		public static EHTTPRequestType MethodEnum (this HttpRequest request)
		{
			return (EHTTPRequestType)Enum.Parse(typeof(EHTTPRequestType), request.Method);
		}
	}
}
