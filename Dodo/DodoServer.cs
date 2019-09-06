// Copyright (C) 2016 by Barend Erasmus, David Jeske and donated to the public domain

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.RouteHandlers;
using System.IO;

namespace XR.Dodo
{
	public static class DodoServer
	{
		public static string SECRET = "xyff39jd1i37";
		public static int Port = 8080;

		public static SiteSpreadsheetManager SiteManager = new SiteSpreadsheetManager("sites.config");

		static void Main(string[] args)
		{
			Initialise(args);
		}

		public static void Initialise(params string[] args)
		{
			SECRET = File.ReadAllText(@"..\..\..\.secret");
			log4net.Config.XmlConfigurator.Configure();

			var route_config = new List<Route>() {
				new Route()
				{
					Name = "SMS Receiver",
					Method = "POST",
					UrlRegex = @"(?:^/)*",
					Callable = (HttpRequest request) =>
					{
						return SMSServer.Read(request);
					}
				}
			};

			HttpServer httpServer = new HttpServer(Port, route_config);

			Thread thread = new Thread(new ThreadStart(httpServer.Listen));
			thread.Start();
		}
	}
}
