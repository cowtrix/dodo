// Copyright (C) 2016 by Barend Erasmus and donated to the public domain

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SimpleHttpServer.Models
{
	public class HttpRequest
	{
		public readonly string Method;
		public readonly string Url;
		public string Path { get; set; }
		public readonly string Content;
		public Route Route { get; set; }
		public readonly Dictionary<string, string> Headers;
		public readonly Dictionary<string, string> QueryParams;

		public HttpRequest(string method, string url, string content, Dictionary<string, string> headers)
		{
			Method = method;
			Url = url;
			Content = content;
			Headers = headers;
			Headers = new Dictionary<string, string>();
			QueryParams = new Dictionary<string, string>();
			if(string.IsNullOrEmpty(Content))
			{
				return;
			}
			var parameters = Content.Split('&');
			foreach (var p in parameters)
			{
				var keyVal = p.Split('=');
				QueryParams.Add(keyVal.First(), HttpUtility.UrlDecode(keyVal.Last()));
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
