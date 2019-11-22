// Copyright (C) 2016 by Barend Erasmus and donated to the public domain

using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SimpleHttpServer.Models
{
	public class HttpRequest
	{
		public readonly EHTTPRequestType Method;
		public readonly string Domain;
		public readonly string Url;
		public string Path { get; set; }
		public readonly string Content;
		public Route Route { get; set; }
		public readonly Dictionary<string, string> Headers;
		public readonly Dictionary<string, string> QueryParams;

		public HttpRequest(string method, string url, string content, Dictionary<string, string> headers)
		{
			if (!Enum.TryParse(method, true, out Method))
			{
				throw new InvalidCastException($"Invalid HTTP method " + method);
			}
			Url = url.Trim('/');
			Domain = Dns.GetHostEntry(Dns.GetHostName()).HostName;
			Content = content;
			Headers = headers;
			QueryParams = new Dictionary<string, string>();
			var paramsIndex = url.IndexOf('?') + 1;
			if(paramsIndex > 0)
			{
				var parameters = url.Substring(paramsIndex).Split('&');
				foreach (var p in parameters)
				{
					var keyVal = p.Split('=');
					QueryParams.Add(keyVal.First(), HttpUtility.UrlDecode(keyVal.Last()));
				}
			}
		}

		public override string ToString()
		{
			if (!string.IsNullOrWhiteSpace(this.Content))
				if (!this.Headers.ContainsKey("Content-Length"))
					this.Headers.Add("Content-Length", this.Content.Length.ToString());

			return string.Format("{0} {1} HTTP/1.0\r\n{2}\r\n\r\n{3}", this.Method, this.Url, string.Join("\r\n", this.Headers.Select(x => string.Format("{0}: {1}", x.Key, x.Value))), this.Content);
		}
	}
}
