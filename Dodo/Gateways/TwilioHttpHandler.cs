﻿using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Linq;
using System.Net;
using Twilio;
using Twilio.TwiML;

namespace XR.Dodo
{
	public class TwilioHttpHandler : HttpGatewayHandler
	{
		public TwilioHttpHandler(Configuration config, HTTPGateway server) : base(config, server)
		{
			AccountSID = config.GatewayData.TwilioSID;
			AccountAuth = config.GatewayData.TwilioAuthToken;
			TwilioClient.Init(AccountSID, AccountAuth);
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
												| SecurityProtocolType.Tls11
												| SecurityProtocolType.Tls12
												| SecurityProtocolType.Ssl3;
		}

		public string AccountSID { get; private set; }
		public string AccountAuth { get; private set; }

		bool IsValid(HttpRequest request)
		{
			if (!request.QueryParams.TryGetValue("AccountSid", out var accountSID) || accountSID != AccountSID)
			{
				return false;
			}
			return true;
		}

		public override HttpResponse Read(HttpRequest request)
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
				var phone = m_server.Phones.SingleOrDefault(x => x.Number == receiverNumber);
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
				var message = new UserMessage(user, body, m_server, fromNumber);
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

		private HttpResponse Failure(string message)
		{
			return new HttpResponse()
			{
				ContentAsUTF8 = $"{{\n    \"payload\":\n    {{\n        \"success\": false,\n        \"error\": {message}\n    }}\n}}",
				ReasonPhrase = "OK",
				StatusCode = "200"
			};
		}
	}
}
