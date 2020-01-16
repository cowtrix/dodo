using Microsoft.AspNetCore.Http;
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
	}
}
