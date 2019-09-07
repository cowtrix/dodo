using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		public SMSMessageResponse(string number, Message message)
		{
			Message = message;
		}
		private readonly Message Message;
		public string UUID { get { return Message.UUID; } }
		public string Number { get { return Message.Owner.PhoneNumber; } }
		public object Value { get { return Message.Content; } }
	}

	public class SMSGateaway : IMessageGateway
	{
		public string SMSSecret = "";

		public SMSGateaway(string secret)
		{
			SMSSecret = secret;
		}

		public Func<Message, UserSession, IEnumerable<Message>> ProcessMessage { get; set; }

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
			var sb = new StringBuilder();
			sb.Append("{\r\n    \"payload\": {\r\n        \"success\": \"true\",\r\n        \"task\": \"send\",\r\n        \"messages\": [\r\n");
			foreach(var response in responses)
			{
				sb.AppendLine("            {");
				sb.AppendLine($"                \"to\": \"{response.Number}\",");
				sb.AppendLine($"                \"message\": \"{response.Value}\",");
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
				// The SMS received is valid and so we can process it
				var fromNumber = request.QueryParams["from"];
				var session = DodoServer.SessionManager.GetOrCreateSessionFromNumber(fromNumber);
				if(session == null)
				{
					return Failure("ERROR 0x34492"); // Number wasn't valid
				}
				var smsmMessage = new SMSMessage()
				{
					From = fromNumber,
					MessageID = request.QueryParams["message_id"],
					DeviceID = request.QueryParams["device_id"],
					ReceiverNumber = request.QueryParams["sent_to"],
					MessageString = request.QueryParams["message"],
					TimeReceived = DateTime.FromFileTimeUtc(long.Parse(request.QueryParams["sent_timestamp"])),
				};
				var message = new Message(session.User, smsmMessage.MessageString);
				var responses = ProcessMessage(message, session);
				var smsResponses = new List<SMSMessageResponse>();
				foreach(var response in responses)
				{
					var smsResponse = new SMSMessageResponse(response.Owner.PhoneNumber, response);
					smsResponses.Add(smsResponse);
				}
				return Reply(smsResponses);
			}
			catch(Exception e)
			{
				Console.WriteLine($"Exception in SMServer.Read: {request?.Content}\n{e.Message}\n{e.StackTrace}");
				return Failure("ERROR 0x34502"); // Incorrect formatting
			}
		}
	}
}
