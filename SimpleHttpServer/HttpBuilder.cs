using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
	/// <summary>
	/// Generate common HTTP Responses
	/// </summary>
	public static class HttpBuilder
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

		public static HttpResponse Forbidden()
		{
			return new HttpResponse()
			{
				ReasonPhrase = "Error",
				StatusCode = "403",
			};
		}

		public static HttpResponse OK()
		{
			return new HttpResponse()
			{
				ReasonPhrase = "OK",
				StatusCode = "200",
			};
		}

		public static HttpResponse OK(object result)
		{
			return new HttpResponse()
			{
				ReasonPhrase = "OK",
				StatusCode = "200",
				ContentAsUTF8 = JsonConvert.SerializeObject(result, Formatting.Indented)
			};
		}

		public static HttpResponse ResourceCreated(object result, string location)
		{
			return new HttpResponse()
			{
				ReasonPhrase = "Created",
				StatusCode = "201",
				Headers = new Dictionary<string, string>()
				{
					{ "Location", location }
				},
				ContentAsUTF8 = JsonConvert.SerializeObject(result, Formatting.Indented)
			};
		}

		public static HttpResponse Custom(string reason, int code)
		{
			return new HttpResponse()
			{
				ReasonPhrase = reason,
				StatusCode = code.ToString(),
			};
		}
	}
}
