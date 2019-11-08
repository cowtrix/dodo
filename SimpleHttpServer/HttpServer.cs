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

	public class HttpServer
	{
		#region Fields

		public static X509Certificate ServerCertificate = null;
		public int Port { get; private set; }
		private HttpListener Listener;
		public bool IsActive = true;

		#endregion

		#region Public Methods
		public HttpServer(int port, List<Route> routes, string certificate, string certificatePassword)
		{
			ServerCertificate = ServerCertificate ?? new X509Certificate2(certificate, certificatePassword);
			Port = port;
			Listener = new HttpListener();
			Listener.Prefixes.Add($"https://+:{port}/");
			//Listener.Prefixes.Add($"http://*:{8080}/");
			/*Processor = new HttpProcessor();

			foreach (var route in routes)
			{
				Processor.AddRoute(route);
			}*/
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
			// New-NetFirewallRule -DisplayName \"My HTTP Listener Print Server\" -Direction Inbound -LocalPort 443 - Protocol TCP - Action Allow
			Listener.Start();
			while (IsActive)
			{
				try
				{
					var context = Listener.GetContext();
					var processTask = new Task(() => ProcessRequest(context));
					processTask.Start();
				}
				catch (Exception e)
				{
					Logger.Exception(e);
				}
			}
		}

		void ProcessRequest(HttpListenerContext context)
		{
			var request = context.Request;
			// Obtain a response object.
			var response = context.Response;
			// Construct a response.
			string responseString = "<HTML><BODY> Hello world!</BODY></HTML>";
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
			// Get a response stream and write the response to it.
			response.ContentLength64 = buffer.Length;
			System.IO.Stream output = response.OutputStream;
			output.Write(buffer, 0, buffer.Length);
			// You must close the output stream.
			output.Close();
		}

		static void HttpListenerCallback(IAsyncResult result)
		{
			HttpListener listener = (HttpListener)result.AsyncState;
			HttpListenerContext context = listener.EndGetContext(result);

			//Process(context);

			var html = $"<html><body><h1>HTTP Listener is working</h1></body></html>";

			byte[] bOutput2 = System.Text.Encoding.UTF8.GetBytes(html);

			context.Response.ContentType = "text/html";
			context.Response.ContentLength64 = bOutput2.Length;
			Stream OutputStream2 = context.Response.OutputStream;
			OutputStream2.Write(bOutput2, 0, bOutput2.Length);
			OutputStream2.Close();
			context.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
		}

		#endregion

	}
}



