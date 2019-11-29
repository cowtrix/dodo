using Common;
using Common.Extensions;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SimpleHttpServer.REST
{
	public class RESTServer
	{
		public const string AUTH_KEY = "Authorization";
		public readonly int Port;
		protected List<Route> Routes = new List<Route>();
		protected MsgReceivedDelegate OnMsgReceieved;
		private static HttpServer m_server;
		private Thread m_serverThread;
		private List<RESTHandler> m_handlers = new List<RESTHandler>();
		private string m_certificatePath;
		private string m_sslPassword;

		public RESTServer(int port, string certificate, string sslPassword)
		{
			if (m_server != null)
			{
				return;
			}
			m_certificatePath = Path.GetFullPath(certificate);
			m_sslPassword = sslPassword;
			if (!File.Exists(m_certificatePath))
			{
				throw new FileNotFoundException("Missing SSL certificate at " + m_certificatePath);
			}
			// Start HTTP server for receiving messages
			Port = port;
			var handlers = ReflectionExtensions.GetChildClasses<RESTHandler>();
			foreach (var handlerType in handlers)
			{
				var handler = Activator.CreateInstance(handlerType) as RESTHandler;
				m_handlers.Add(handler);
				handler.AddRoutes(Routes);
			}
		}

		public void Start()
		{
			m_server = new HttpServer(Port, Routes, m_certificatePath, m_sslPassword);
			m_serverThread = new Thread(new ThreadStart(m_server.Listen));
			m_serverThread.Start();
			m_server.OnMsgReceived += OnMsgReceieved;
		}

		public void Shutdown()
		{
			m_server.IsActive = false;
		}
	}
}
