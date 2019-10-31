﻿using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
	public class HttpBuilder
	{
		public static HttpResponse InternalServerError()
		{
			string content = File.ReadAllText("Resources/Pages/500.html");

			return new HttpResponse()
			{
				ReasonPhrase = "InternalServerError",
				StatusCode = "500",
				ContentAsUTF8 = content
			};
		}

		public static HttpResponse NotFound()
		{
			string content = File.ReadAllText("Resources/Pages/404.html");

			return new HttpResponse()
			{
				ReasonPhrase = "NotFound",
				StatusCode = "404",
				ContentAsUTF8 = content
			};
		}

		public static HttpResponse Error(string errorMessage, int errorCode = 200)
		{
			return new HttpResponse()
			{
				ReasonPhrase = "Error",
				StatusCode = errorCode.ToString(),
				ContentAsUTF8 = errorMessage
			};
		}
	}
}
