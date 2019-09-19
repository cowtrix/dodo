using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;

namespace XR.Dodo
{
	public class Phone
	{
		public enum ESMSMode
		{
			Verification,
			Bot,
		}
		public ESMSMode Mode;
		public string Name;
		public string Number;
	}

	public class SMSGateway : IMessageGateway
	{
		public string AccountSID { get; private set; }
		public string AccountAuth { get; private set; }

		public List<Phone> Phones = new List<Phone>();
		public readonly int Port;
		const string m_filePath = "Backups\\smsNumbers.json";
		private static HttpServer m_server;
		private Thread m_serverThread;

		public EGatewayType Type { get { return EGatewayType.Twilio; } }

		public SMSGateway(string accountSID, string authToken, int port)
		{
			// Set up connection to send SMS messages
			AccountSID = accountSID;
			AccountAuth = authToken;
			TwilioClient.Init(AccountSID, AccountAuth);
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
												| SecurityProtocolType.Tls11
												| SecurityProtocolType.Tls12
												| SecurityProtocolType.Ssl3;
			// Add phones (TODO: load from file)
			Phones.Add(new Phone()
			{
				Number = "",
				Name = "Trial",
				Mode = Phone.ESMSMode.Verification,
			});

			if (m_server != null)
			{
				return;
			}

			// Start HTTP server for receiving messages
			Port = port;
			var route_config = new List<Route>() {
				new Route()
				{
					Name = "SMS Receiver",
					Method = "POST",
					UrlRegex = @"(?:^/)*",
					Callable = (HttpRequest request) =>
					{
						try
						{
							return Read(request);
						}
						catch(Exception e)
						{
							Logger.Exception(e);
						}
						return Failure("404");
					}
				}
			};
			m_server = new HttpServer(Port, route_config);
			m_serverThread = new Thread(new ThreadStart(m_server.Listen));
			m_serverThread.Start();
		}

		HttpResponse Failure(string message)
		{
			return new HttpResponse()
			{
				ContentAsUTF8 = $"{{\n    \"payload\":\n    {{\n        \"success\": false,\n        \"error\": {message}\n    }}\n}}",
				ReasonPhrase = "OK",
				StatusCode = "200"
			};
		}

		public Phone GetPhone()
		{
			return Phones.Random();
		}

		public Phone GetPhone(Phone.ESMSMode mode)
		{
			return Phones.Where(x => x.Mode == mode).Random();
		}

		bool IsValid(HttpRequest request)
		{
			if (!request.QueryParams.TryGetValue("AccountSid", out var accountSID) || accountSID != AccountSID)
			{
				return false;
			}
			return true;
		}

		public HttpResponse Read(HttpRequest request)
		{
			try
			{
				if (!IsValid(request))
				{
					return HttpBuilder.NotFound();
				}
				var receiverNumber = request.QueryParams["To"];
				if (!ValidationExtensions.ValidateNumber(ref receiverNumber))
				{
					return Failure("4042");
				}
				var phone = Phones.SingleOrDefault(x => x.Number == receiverNumber);
				if (phone == null)
				{
					Logger.Error("Unrecognized origin number: " + receiverNumber);
					return Failure("4044");
				}

				// The SMS received is valid and so we can process it
				var fromNumber = request.QueryParams["From"];
				if (!ValidationExtensions.ValidateNumber(ref fromNumber))
				{
					return Failure("4046");
				}
				var body = request.QueryParams["Body"];

				if (phone.Mode == Phone.ESMSMode.Verification)
				{
					DodoServer.SessionManager.TryVerify(fromNumber, body);
					return Success();
				}

				var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(fromNumber);
				var message = new UserMessage(user, body, this, fromNumber);
				var session = DodoServer.SessionManager.GetOrCreateSession(user);
				if (session == null)
				{
					return Failure("ERROR 0x34492"); // Number wasn't valid
				}
				var response = session.ProcessMessage(message, session);
				return Reply(response, session);
			}
			catch (Exception e)
			{
				Logger.Exception(e, $"Exception in SMServer.Read: {request?.Content}");
				return Failure("ERROR 0x34502"); // Incorrect formatting
			}
		}

		private HttpResponse Reply(ServerMessage response, UserSession session)
		{
			var messagingResponse = new MessagingResponse();
			messagingResponse.Message(response.Content);
			return new HttpResponse()
			{
				ContentAsUTF8 = messagingResponse.ToString(),
				ReasonPhrase = "OK",
				StatusCode = "200"
			};
		}

		private HttpResponse Success()
		{
			var messagingResponse = new MessagingResponse();
			return new HttpResponse()
			{
				ContentAsUTF8 = messagingResponse.ToString(),
				ReasonPhrase = "OK",
				StatusCode = "200"
			};
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
