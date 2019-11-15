// Copyright (C) 2016 by Barend Erasmus and donated to the public domain

using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleHttpServer.Models
{
	public class Route
	{
		public string Name { get; set; } // descriptive name for debugging
		public Func<string, bool> UrlMatcher { get; set; }
		public EHTTPRequestType Method { get; set; }
		public Func<HttpRequest, HttpResponse> Callable { get; set; }

		public Route(string name, EHTTPRequestType method, Func<string, bool> urlMatcher, Func<HttpRequest, HttpResponse> callback)
		{
			Name = name;
			Method = method;
			UrlMatcher = urlMatcher;
			Callable = callback;
		}
	}
}
