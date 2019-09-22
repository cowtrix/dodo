using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
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
	public class Phone
	{
		public enum ESMSMode
		{
			Verification,
			Bot,
		}
		public enum EType
		{
			SMSSync,
			Twilio,
		}
		public EType Type;
		public ESMSMode Mode;
		public string Name;
		public string Number;
	}

	public abstract class HttpGatewayHandler
	{
		protected HTTPGateway m_server;
		public HttpGatewayHandler(Configuration config, HTTPGateway server)
		{
			m_server = server;
		}
		public abstract HttpResponse Read(HttpRequest request);
	}

	public class HTTPGateway : IMessageGateway
	{
		public List<Phone> Phones = new List<Phone>();
		public readonly int Port;
		private static HttpServer m_server;
		private Thread m_serverThread;

		public EGatewayType Type { get { return EGatewayType.SMS; } }
		private TwilioHttpHandler m_twilioHandler;
		private SMSSyncHttpHandler m_smsSyncHandler;

		public HTTPGateway(Configuration config)
		{
			Phones = config.GatewayData.Phones;
			m_twilioHandler = new TwilioHttpHandler(config, this);
			m_smsSyncHandler = new SMSSyncHttpHandler(config, this);

			if (m_server != null)
			{
				return;
			}

			// Start HTTP server for receiving messages
			Port = config.GatewayData.HTTPServerPort;
			var route_config = new List<Route>() {
				new Route()
				{
					Name = "Twilio Receiver",
					Method = "POST",
					UrlRegex = @"/twilio(?:^/)*",
					Callable = (HttpRequest request) =>
					{
						try
						{
							return m_twilioHandler.Read(request);
						}
						catch(Exception e)
						{
							Logger.Exception(e);
						}
						return new HttpResponse()
						{
							StatusCode = "404",
						};
					}
				},
				new Route()
				{
					Name = "SMSSync Receiver",
					Method = "POST",
					UrlRegex = @"/sync(?:^/)*",
					Callable = (HttpRequest request) =>
					{
						try
						{
							return m_smsSyncHandler.Read(request);
						}
						catch(Exception e)
						{
							Logger.Exception(e);
						}
						return new HttpResponse()
						{
							StatusCode = "404",
						};
					}
				}
			};
			m_server = new HttpServer(Port, route_config);
			m_serverThread = new Thread(new ThreadStart(m_server.Listen));
			m_serverThread.Start();
		}


		public Phone GetPhone()
		{
			return Phones.Random();
		}

		public Phone GetPhone(Phone.ESMSMode mode)
		{
			return Phones.Where(x => x.Mode == mode).Random();
		}

		public void Shutdown()
		{
			m_server.IsActive = false;
		}

		public void SendMessage(ServerMessage message, UserSession session)
		{
			SendMessage(message, GetPhone(Phone.ESMSMode.Bot), session);
		}

		public void SendMessage(ServerMessage message, Phone origin, UserSession session)
		{
			var msgResource = MessageResource.Create(
				body: message.Content,
				from: new Twilio.Types.PhoneNumber(origin.Number),
				to: new Twilio.Types.PhoneNumber("+" + session.GetUser().PhoneNumber)
			);
		}

		public void SendMessage(ServerMessage message, Phone origin, string targetNumber)
		{
			var msgResource = MessageResource.Create(
				body: message.Content,
				from: new Twilio.Types.PhoneNumber(origin.Number),
				to: new Twilio.Types.PhoneNumber("+" + targetNumber)
			);
		}

		public ServerMessage FakeMessage(string msg, string phone)
		{
			var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(phone);
			var message = new UserMessage(user, msg, this, phone);
			var session = DodoServer.SessionManager.GetOrCreateSession(user);
			return session.ProcessMessage(message, session);
		}
	}
}
