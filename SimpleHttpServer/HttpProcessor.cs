// Copyright (C) 2016 by David Jeske, Barend Erasmus and donated to the public domain

using Common;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SimpleHttpServer
{
	public class HttpProcessor
	{

		#region Fields

		private List<Route> Routes = new List<Route>();

		#endregion

		#region Constructors

		public HttpProcessor()
		{
		}

		#endregion

		#region Public Methods
		public void HandleClient(TcpClient client)
		{
			// A client has connected. Create the
			// SslStream using the client's network stream.
			var sslStream = new SslStream(client.GetStream(), false);
			// Authenticate the server but don't require the client to authenticate.
			try
			{
				sslStream.AuthenticateAsServer(HttpServer.ServerCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);
				// Set timeouts for the read and write to 5 seconds.
				sslStream.ReadTimeout = 500000;			// TODO set to sane values
				sslStream.WriteTimeout = 500000;
				// Read a message from the client.
				var request = GetRequest(sslStream);

				// Write a message to the client.
				var response = RouteRequest(request);
				WriteResponse(sslStream, response);
			}
			catch (AuthenticationException e)
			{
				Logger.Exception(e, "Authentication failed - closing the connection.");
				sslStream.Close();
				client.Close();
				return;
			}
			finally
			{
				// The client stream will be closed with the sslStream
				// because we specified this behavior when creating
				// the sslStream.
				sslStream.Close();
				client.Close();
			}
		}
		public void AddRoute(Route route)
		{
			this.Routes.Add(route);
		}

		#endregion

		#region Private Methods
		static char[] ReadFromSSlStream(SslStream sslStream, int totalSize = 1)
		{
			int chunkSize = 2048;
			byte[] buffer = new byte[chunkSize];
			List<char> result = new List<char>();
			while(totalSize > 0)
			{
				var bytes = sslStream.Read(buffer, 0, buffer.Length);
				totalSize -= bytes;
				// Use Decoder class to convert from bytes to UTF8
				// in case a character spans two buffers.
				Decoder decoder = Encoding.UTF8.GetDecoder();
				char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
				decoder.GetChars(buffer, 0, bytes, chars, 0);
				result.AddRange(chars);
			}
			return result.ToArray();
		}

		private static void WriteResponse(Stream stream, HttpResponse response)
		{
			if (response.Content == null)
			{
				response.Content = new byte[] { };
			}

			// default to text/html content type
			if (!response.Headers.ContainsKey("Content-Type"))
			{
				response.Headers["Content-Type"] = "text/html";
			}

			response.Headers["Content-Length"] = response.Content.Length.ToString();

			Write(stream, string.Format("HTTP/1.0 {0} {1}\r\n", response.StatusCode, response.ReasonPhrase));
			Write(stream, string.Join("\r\n", response.Headers.Select(x => string.Format("{0}: {1}", x.Key, x.Value))));
			Write(stream, "\r\n\r\n");

			stream.Write(response.Content, 0, response.Content.Length);
		}

		private static string Readline(Stream stream)
		{
			int next_char;
			string data = "";
			while (true)
			{
				next_char = stream.ReadByte();
				if (next_char == '\n') { break; }
				if (next_char == '\r') { continue; }
				if (next_char == -1) { Thread.Sleep(1); continue; };
				data += Convert.ToChar(next_char);
			}
			return data;
		}

		private static void Write(Stream stream, string text)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			stream.Write(bytes, 0, bytes.Length);
		}

		protected virtual HttpResponse RouteRequest(HttpRequest request)
		{
			List<Route> routes = this.Routes.Where(x => x.UrlMatcher(request.Url)).ToList();

			if (!routes.Any())
				return HttpBuilder.NotFound();

			Route route = routes.SingleOrDefault(x => x.Method == request.Method);

			if (route == null)
				return new HttpResponse()
				{
					ReasonPhrase = "Method Not Allowed",
					StatusCode = "405",

				};

			request.Path = request.Url;

			// trigger the route handler...
			request.Route = route;
			try {
				return route.Callable(request);
			} catch(Exception ex) {
				Logger.Exception(ex);
				return HttpBuilder.InternalServerError();
			}
		}

		private static HttpRequest GetRequest(SslStream sslStream)
		{
			// Read the  message sent by the client.
			using (var inputStream = new MemoryStream())
			using (var streamwriter = new StreamWriter(inputStream))
			{
				var chars = ReadFromSSlStream(sslStream);
				streamwriter.Write(chars, 0, chars.Length);
				streamwriter.Flush();
				inputStream.Position = 0;

				//Read Request Line
				string request = Readline(inputStream);

				string[] tokens = request.Split(' ');
				if (tokens.Length != 3)
				{
					throw new Exception("invalid http request line");
				}
				string method = tokens[0].ToUpper();
				string url = tokens[1];
				string protocolVersion = tokens[2];

				//Read Headers
				Dictionary<string, string> headers = new Dictionary<string, string>();
				string line;
				while ((line = Readline(inputStream)) != null)
				{
					if (line.Equals(""))
					{
						break;
					}

					int separator = line.IndexOf(':');
					if (separator == -1)
					{
						throw new Exception("invalid http header line: " + line);
					}
					string name = line.Substring(0, separator);
					int pos = separator + 1;
					while ((pos < line.Length) && (line[pos] == ' '))
					{
						pos++;
					}

					string value = line.Substring(pos, line.Length - pos);
					headers.Add(name, value);
				}
				string content = null;
				if (headers.ContainsKey("Content-Length"))
				{
					int totalBytes = Convert.ToInt32(headers["Content-Length"]);
					int bytesLeft = totalBytes;
					// TODO handle chunked requests better than the below does
					if (inputStream.Length < inputStream.Position + totalBytes)
					{
						content = new string(ReadFromSSlStream(sslStream, bytesLeft));
					}
					else
					{
						byte[] bytes = new byte[totalBytes];
						while (bytesLeft > 0)
						{
							byte[] buffer = new byte[bytesLeft > 1024 ? 1024 : bytesLeft];
							int n = inputStream.Read(buffer, 0, buffer.Length);
							if (n == 0)
							{
								throw new Exception("Unexpected end of stream");
							}
							buffer.CopyTo(bytes, totalBytes - bytesLeft);
							bytesLeft -= n;
						}
						content = Encoding.ASCII.GetString(bytes);
					}
				}
				return new HttpRequest(method, url, content, headers);
			}
		}

		#endregion


	}
}
