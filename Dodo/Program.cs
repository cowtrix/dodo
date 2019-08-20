// Copyright (C) 2016 by Barend Erasmus, David Jeske and donated to the public domain

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.RouteHandlers;

namespace XR.Dodo
{
    class Program
    {
        public static string SECRET = "test";

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var route_config = new List<Route>() {
                new Route()
                {
                    Name = "SMS Receiver",
                    Method = "POST",
                    UrlRegex = @"(?:^/)*",
                    Callable = (HttpRequest request) => 
                    {
                        return SMSReader.Read(request);
                     }
                }
            };

            HttpServer httpServer = new HttpServer(8080, route_config);
            
            Thread thread = new Thread(new ThreadStart(httpServer.Listen));
            thread.Start();
        }
    }
}
