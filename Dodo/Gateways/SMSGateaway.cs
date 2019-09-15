using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace XR.Dodo
{
	public struct SMSMessage
	{
		public string MessageString;
		public DateTime TimeReceived;
		public string MessageID;
		public string ReceiverNumber;
		public string DeviceID;
		public string From;
	}

	public readonly struct SMSMessageResponse
	{
		public SMSMessageResponse(string targetNumber, ServerMessage message)
		{
			Message = message;
			TargetNumber = targetNumber;
		}
		public readonly ServerMessage Message;
		public readonly string TargetNumber;
		public string UUID { get { return Message.MessageID; } }
	}
	
	public class SMSGateaway : IMessageGateway
	{
		public struct Phone
		{
			public string Name;
			public string Number;
			public int OutboxCount;
			public DateTime LastMessage;
		}

		public List<Phone> Phones = new List<Phone>();

		public readonly int Port;
		public string SMSSecret = "";
		const string m_filePath = "smsNumbers.json";

		public SMSGateaway(string secret, int port)
		{
			if (File.Exists(m_filePath))
			{
				Phones = JsonConvert.DeserializeObject<List<Phone>>(File.ReadAllText(m_filePath))
					?? Phones;
			}
			Phones.Add(new Phone()
			{
				Name = "My phone",
				Number = "+447856465191"
			});
			SMSSecret = secret;
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
						return Failure("An error occurred");
					}
				}
			};
			var httpServer = new HttpServer(Port, route_config);
			var thread = new Thread(new ThreadStart(httpServer.Listen));
			thread.Start();
		}

		public string GetNumber()
		{
			return Phones.Random().Number;
		}

		bool IsValid(HttpRequest request)
		{
			if (!request.QueryParams.TryGetValue("secret", out var secret) || secret != SMSSecret)
			{
				return false;
			}
			return true;
		}

		HttpResponse Success ()
		{
			return new HttpResponse()
			{
				ContentAsUTF8 = "{\n    \"payload\":\n    {\n        \"success\": true,\n        \"error\": null\n    }\n}",
				ReasonPhrase = "OK",
				StatusCode = "200"
			};
		}

		HttpResponse Reply(SMSMessageResponse response)
		{
			return Reply(new[] { response });
		}

		HttpResponse Reply(IEnumerable<SMSMessageResponse> responses)
		{
			responses = responses.Where(x => !string.IsNullOrEmpty(x.Message.Content));
			if(responses.Count() == 0)
			{
				return Success();
			}
			var sb = new StringBuilder();
			sb.Append("{\r\n    \"payload\": {\r\n        \"success\": \"true\",\r\n        \"task\": \"send\",\r\n        \"messages\": [\r\n");
			foreach(var response in responses)
			{
				sb.AppendLine("            {");
				sb.AppendLine($"                \"to\": \"{response.TargetNumber}\",");
				sb.AppendLine($"                \"message\": \"{response.Message.Content}\",");
				sb.AppendLine($"                \"uuid\": \"{response.UUID}\"");
				sb.AppendLine("            }");
				if(response.UUID != responses.Last().UUID)
				{
					sb.Append(",");
				}
			}
			sb.Append("        ]\r\n    }\r\n}");
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
				ContentAsUTF8 = $"{{\n    \"payload\":\n    {{\n        \"success\": false,\n        \"error\": {message}\n    }}\n}}",
				ReasonPhrase = "OK",
				StatusCode = "200"
			};
		}

		public HttpResponse Read(HttpRequest request)
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
					return Failure("Invalid receiver number: " + request.QueryParams["sent_to"]);
				}
				// The SMS received is valid and so we can process it
				var fromNumber = request.QueryParams["from"];
				var smsmMessage = new SMSMessage()
				{
					From = fromNumber,
					MessageID = request.QueryParams["message_id"],
					DeviceID = request.QueryParams["device_id"],
					ReceiverNumber = receiverNumber,
					MessageString = request.QueryParams["message"],
					TimeReceived = DateTime.FromFileTimeUtc(long.Parse(request.QueryParams["sent_timestamp"])),
				};

				var user = DodoServer.SessionManager.GetOrCreateUserFromPhoneNumber(fromNumber, smsmMessage.MessageString);
				var message = new UserMessage(user, smsmMessage.MessageString, EGatewayType.SMS,
					smsmMessage.From);
				
				var session = DodoServer.SessionManager.GetOrCreateSession(user);
				if (session == null)
				{
					return Failure("ERROR 0x34492"); // Number wasn't valid
				}
				var response = session.ProcessMessage(message, session);
				var smsResponse = new SMSMessageResponse(user.PhoneNumber, response);
				return Reply(smsResponse);
			}
			catch(Exception e)
			{
				Logger.Exception(e, $"Exception in SMServer.Read: {request?.Content}");
				return Failure("ERROR 0x34502"); // Incorrect formatting
			}
		}

		public void SendMessage(ServerMessage message, UserSession session)
		{
			throw new NotImplementedException();
		}
	}
}
