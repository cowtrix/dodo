using Common;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace XR.Dodo
{
	public enum EHTTPRequestType
	{
		GET,
		POST,
		PUT,
		DELETE,
	}

	public class HTTPServer
	{
		public readonly int Port;
		private static HttpServer m_server;
		private Thread m_serverThread;

		public HTTPServer(Configuration config)
		{
			if (m_server != null)
			{
				return;
			}
			// Start HTTP server for receiving messages
			Port = config.GatewayData.HTTPServerPort;
			var handlers = ReflectionExtensions.GetChildClasses<RESTHandler>();
			var routeConfig = new List<Route>();
			foreach (var handler in handlers)
			{
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
