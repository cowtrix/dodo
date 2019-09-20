// Copyright (C) 2016 by David Jeske, Barend Erasmus and donated to the public domain

using log4net;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace SimpleHttpServer
{

	public class HttpServer
	{
		#region Fields

		public int Port { get; private set; }
		private TcpListener Listener;
		private HttpProcessor Processor;
		public bool IsActive = true;

		#endregion

		private static readonly ILog log = LogManager.GetLogger(typeof(HttpServer));

		#region Public Methods
		public HttpServer(int port, List<Route> routes)
		{
			this.Port = port;
			this.Processor = new HttpProcessor();

			foreach (var route in routes)
			{
				this.Processor.AddRoute(route);
			}
		}

		public static string GetLocalIPAddress()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			foreach (var ip in host.AddressList)
			{
				if (ip.AddressFamily == AddressFamily.InterNetwork)
				{
					return ip.ToString();
				}
			}
			throw new Exception("No network adapters with an IPv4 address in the system!");
		}

		public void Listen()
		{
			Console.WriteLine($"Started HTTP server at {GetLocalIPAddress()}:{Port}");

			this.Listener = new TcpListener(IPAddress.Any, this.Port);
			this.Listener.Start();
			while (this.IsActive)
			{
				TcpClient s = this.Listener.AcceptTcpClient();
				Thread thread = new Thread(() =>
				{
					this.Processor.HandleClient(s);
				});
				thread.Start();
				Thread.Sleep(1);
			}
		}

		#endregion

	}
}



