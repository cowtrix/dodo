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
	/// A utility class to generate common HTTP Responses.
	/// Server code meanings taken from: https://www.w3.org/Protocols/rfc2616/rfc2616-sec10.html
	/// </summary>
	public static class HttpBuilder
	{
		/// <summary>
		/// The server has not found anything matching the Request-URI.
		/// </summary>
		/// <returns>An HttpResponse object with this response code</returns>
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

		/// <summary>
		/// The server encountered an unexpected condition which prevented it from fulfilling the request.
		/// </summary>
		/// <param name="errorMessage">A description of the error that occurred</param>
		/// <returns>An HttpResponse object with this response code</returns>
		public static HttpResponse ServerError(string errorMessage)
		{
			return new HttpResponse()
			{
				ReasonPhrase = "Internal Server Error",
				StatusCode = "500",
				ContentAsUTF8 = errorMessage
			};
		}

		/// <summary>
		/// The server understood the request, but is refusing to fulfill it. Authorization will not help and the request SHOULD NOT be repeated.
		/// </summary>
		/// <returns>An HttpResponse object with this response code</returns>
		public static HttpResponse Forbidden()
		{
			return new HttpResponse()
			{
				ReasonPhrase = "Error",
				StatusCode = "403",
			};
		}

		/// <summary>
		/// The request has succeeded.
		/// </summary>
		/// <returns>An HttpResponse object with this response code</returns>
		public static HttpResponse OK()
		{
			return new HttpResponse()
			{
				ReasonPhrase = "OK",
				StatusCode = "200",
			};
		}

		/// <summary>
		/// The request has succeeded. Encode a given object to JSON as the response body.
		/// </summary>
		/// <param name="result">The object to convert to JSON and trasmit</param>
		/// <returns>An HttpResponse object with this response code, and the given object as JSON</returns>
		public static HttpResponse OK(object result)
		{
			return new HttpResponse()
			{
				ReasonPhrase = "OK",
				StatusCode = "200",
				ContentAsUTF8 = JsonConvert.SerializeObject(result, Formatting.Indented)
			};
		}

		/// <summary>
		/// The request has been fulfilled and resulted in a new resource being created.
		/// </summary>
		/// <param name="result">The object that has been created</param>
		/// <param name="location">The Resource URI of the object that was created</param>
		/// <returns>An HttpResponse object with this response code, and the given object as JSON</returns>
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

		/// <summary>
		/// Send a custom HttpResponse with the given reason and code
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="code"></param>
		/// <returns></returns>
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
