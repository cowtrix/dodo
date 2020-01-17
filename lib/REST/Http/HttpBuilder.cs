using Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace REST
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
		public static NotFoundResult NotFound()
		{
			return new NotFoundResult();
		}

		/// <summary>
		/// The server encountered an unexpected condition which prevented it from fulfilling the request.
		/// </summary>
		/// <param name="errorMessage">A description of the error that occurred</param>
		/// <returns>An HttpResponse object with this response code</returns>
		public static StatusCodeResult ServerError(string errorMessage)
		{
			return new StatusCodeResult((int)HttpStatusCode.InternalServerError)
			{
				//Content = new StringContent(errorMessage), 
			};
		}

		/// <summary>
		/// The server understood the request, but is refusing to fulfill it. Authorization will not help and the request SHOULD NOT be repeated.
		/// </summary>
		/// <returns>An HttpResponse object with this response code</returns>
		public static ForbidResult Forbidden()
		{
			return new ForbidResult();
		}

		/// <summary>
		/// The request has succeeded.
		/// </summary>
		/// <returns>An HttpResponse object with this response code</returns>
		public static OkResult OK()
		{
			return new OkResult();
		}

		/// <summary>
		/// The request has succeeded. Encode a given object to JSON as the response body.
		/// </summary>
		/// <param name="result">The object to convert to JSON and trasmit</param>
		/// <returns>An HttpResponse object with this response code, and the given object as JSON</returns>
		public static OkObjectResult OK(object result)
		{
			return new OkObjectResult(result);
		}

		/// <summary>
		/// The request has been fulfilled and resulted in a new resource being created.
		/// </summary>
		/// <param name="result">The object that has been created</param>
		/// <param name="location">The Resource URI of the object that was created</param>
		/// <returns>An HttpResponse object with this response code, and the given object as JSON</returns>
		public static CreatedResult ResourceCreated(object result, string location)
		{
			return new CreatedResult(location, result);
		}

		/// <summary>
		/// Send a custom HttpResponse with the given reason and code
		/// </summary>
		/// <param name="reason"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public static HttpStatusContentResult Custom(string reason, HttpStatusCode code)
		{
			return new HttpStatusContentResult(code, reason);
		}
	}
}
