﻿using Common;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo.Dodo
{
	public enum EHTTPRequestType
	{
		GET,
		POST,
		PUT,
		DELETE,
	}

	public class RESTServer
	{
		public readonly int Port;
		private static HttpServer m_server;
		private Thread m_serverThread;
		private List<RESTHandler> m_handlers = new List<RESTHandler>();

		public RESTServer(int port)
		{
			if (m_server != null)
			{
				return;
			}
			// Start HTTP server for receiving messages
			Port = port;
			var handlers = ReflectionExtensions.GetChildClasses<RESTHandler>();
			var routeConfig = new List<Route>();
			foreach (var handlerType in handlers)
			{
				var handler = Activator.CreateInstance(handlerType) as RESTHandler;
				m_handlers.Add(handler);
				handler.AddRoutes(routeConfig);
			}
			m_server = new HttpServer(Port, routeConfig);
			m_serverThread = new Thread(new ThreadStart(m_server.Listen));
			m_serverThread.Start();
		}

		public void Shutdown()
		{
			m_server.IsActive = false;
		}
	}
}