using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	public class SMSSyncHttpHandler : HttpGatewayHandler
	{
		public string SmsSyncSecret { get; private set; }

		public SMSSyncHttpHandler(Configuration config, HTTPGateway server) : base(config, server)
		{
			SmsSyncSecret = config.GatewayData.SmsSyncSecret;
		}

		HttpResponse Success()
		{
			return new HttpResponse()
			{
				ContentAsUTF8 = "{\n	\"payload\":\n	{\n		\"success\": true,\n		\"error\": null\n	}\n}",
				ReasonPhrase = "OK",
				StatusCode = "200"
			};
		}

		HttpResponse Reply(ServerMessage response, UserSession session)
		{
			return Reply(new[] { response }, session);
		}

		HttpResponse Reply(IEnumerable<ServerMessage> responses, UserSession session)
		{
			responses = responses.Where(x => !string.IsNullOrEmpty(x.Content));
			if (responses.Count() == 0)
			{
				return Success();
			}
			var sb = new StringBuilder();
			sb.Append("{\r\n	\"payload\": {\r\n		\"success\": \"true\",\r\n		\"task\": \"send\",\r\n		\"messages\": [\r\n");
			foreach (var response in responses)
			{
				sb.AppendLine("			{");
				sb.AppendLine($"				\"to\": \"{session.GetUser().PhoneNumber}\",");
				sb.AppendLine($"				\"message\": \"{response.Content}\",");
				sb.AppendLine($"				\"uuid\": \"{response.MessageID}\"");
				sb.AppendLine("			}");
				if (response.MessageID != responses.Last().MessageID)
				{
					sb.Append(",");
				}
			}
			sb.Append("		]\r\n	}\r\n}");
			return new HttpResponse()
			{
				ContentAsUTF8 = sb.ToString(),
				ReasonPhrase = "OK",
				StatusCode = "200",
			};
		}

		HttpResponse Failure(string message)
		{
			return new HttpResponse()
			{
				ContentAsUTF8 = $"{{\n	\"payload\":\n	{{\n		\"success\": false,\n		\"error\": {message}\n	}}\n}}",
				ReasonPhrase = "OK",
				StatusCode = "200"
			};
		}

		bool IsValid(HttpRequest request)
		{
			if (!request.QueryParams.TryGetValue("secret", out var secret) || secret != SmsSyncSecret)
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
				var receiverNumber = request.QueryParams["sent_to"];
				if (!ValidationExtensions.ValidateNumber(ref receiverNumber))
				{
					return Failure("4042");
				}

				var body = request.QueryParams["message"];
				var phone = m_server.Phones.FirstOrDefault(x => x.Type == Phone.EType.SMSSync);
				if (phone == null)
				{
					Logger.Error($"Unrecognized origin number: {receiverNumber} with message: {body}");
					return Success();
				}

				// The SMS received is valid and so we can process it
				var fromNumber = request.QueryParams["from"];
				if (!ValidationExtensions.ValidateNumber(ref fromNumber))
				{
					return Failure("4046");
				}

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
	}
			};
