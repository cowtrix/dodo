using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace REST
{
	public class Route
	{
		public string Name { get; set; } // descriptive name for debugging
		public Func<string, bool> UrlMatcher { get; set; }
		public EHTTPRequestType Method { get; set; }
		public Func<HttpRequest, IActionResult> Callable { get; set; }

		public Route(string name, EHTTPRequestType method, Func<string, bool> urlMatcher, Func<HttpRequest, IActionResult> callback)
		{
			Name = name;
			Method = method;
			UrlMatcher = urlMatcher;
			Callable = callback;
		}
	}
}
