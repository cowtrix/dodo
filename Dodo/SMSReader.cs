using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	public struct Message
	{
		public string MessageString;
		public DateTime TimeReceived;
		public string MessageID;
		public string ReceiverNumber;
		public string DeviceID;
		public string From;
	}

	public readonly struct MessageResponse
	{
		public MessageResponse(string number, string message)
		{
			Number = number;
			Message = message;
			UUID = Guid.NewGuid().ToString();
		}

		public readonly string Number;
		public readonly string Message;
		public readonly string UUID;
	}

	static class SMSServer
	{
		static Dictionary<string, UserSession> _sessions = new Dictionary<string, UserSession>();

		static bool IsValid(HttpRequest request)
		{
			if (!request.QueryParams.TryGetValue("secret", out var secret) || secret != Program.SECRET)
			{
				return false;
			}
			return true;
		}

		static HttpResponse Success { get
			{
				return new HttpResponse()
				{
					ContentAsUTF8 = "{\n    \"payload\":\n    {\n        \"success\": true,\n        \"error\": null\n    }\n}",
					ReasonPhrase = "OK",
					StatusCode = "200"
				};
			}
		}

		static HttpResponse Reply(MessageResponse response)
		{
			return Reply(new[] { response });
		}

		static HttpResponse Reply(IEnumerable<MessageResponse> responses)
		{
			var sb = new StringBuilder();
			sb.Append("{\r\n    \"payload\": {\r\n        \"success\": \"true\",\r\n        \"task\": \"send\",\r\n        \"messages\": [\r\n");
			foreach(var response in responses)
			{
				sb.AppendLine("            {");
				sb.AppendLine($"                \"to\": \"{response.Number}\",");
				sb.AppendLine($"                \"message\": \"{response.Message}\",");
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

		static HttpResponse Failure(string message)
		{
			return new HttpResponse()
			{
				ContentAsUTF8 = $"{{\n    \"payload\":\n    {{\n        \"success\": false,\n        \"error\": {message}\n    }}\n}}",
				ReasonPhrase = "OK",
				StatusCode = "200"
			};
		}

		internal static HttpResponse Read(HttpRequest request)
		{
			if(!IsValid(request))
			{
				return HttpBuilder.NotFound();
			}
			// The SMS received is valid and so we can process it
			var fromNumber = request.QueryParams["from"];
			if (!_sessions.TryGetValue(fromNumber, out var session))
			{
				session = new UserSession(fromNumber);
				_sessions[fromNumber] = session;
			}
			var message = new Message()
			{
				From = fromNumber,
				MessageID = request.QueryParams["message_id"],
				DeviceID = request.QueryParams["device_id"],
				ReceiverNumber = request.QueryParams["sent_to"],
				MessageString = request.QueryParams["message"],
				TimeReceived = DateTime.FromFileTimeUtc(long.Parse(request.QueryParams["sent_timestamp"])),
			};
			session.Messages.Add(message);
			var response = new MessageResponse(message.From, $"I received your message! You said: {message.MessageString}");
			session.Responses.Add(response);
			return Reply(response);
		}
	}
}
