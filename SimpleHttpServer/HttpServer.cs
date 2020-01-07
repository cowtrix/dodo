// Copyright (C) 2016 by David Jeske, Barend Erasmus and donated to the public domain

using Common;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
	public delegate void MsgReceivedDelegate(HttpRequest request);
	public class HttpServer
	{
		#region Fields
		public MsgReceivedDelegate OnMsgReceived;
		public static X509Certificate ServerCertificate = null;
		public int Port { get; private set; }
		private TcpListener Listener;
		private HttpProcessor Processor;
		public bool IsActive = true;
		#endregion

		#region Public Methods
		public HttpServer(int port, List<Route> routes, string certificate, string certificatePassword)
		{
			ServerCertificate = ServerCertificate ?? new X509Certificate2(certificate, certificatePassword);
			Port = port;
			Processor = new HttpProcessor(this);

			foreach (var route in routes)
			{
				Processor.AddRoute(route);
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

		public async Task Listen()
		{
			Console.WriteLine($"Started HTTP server at {GetLocalIPAddress()}:{Port}");

			Listener = new TcpListener(IPAddress.Any, Port);
			Listener.Start();
			while (IsActive)
			{
				TcpClient s = await this.Listener.AcceptTcpClientAsync();
				Thread thread = new Thread(() =>
				{
					try
					{
						Processor.HandleClient(s);
					}
					catch(Exception e)
					{
						Logger.Exception(e);
					}
				});
				thread.Start();
			}
		}

		#endregion

	}
}



