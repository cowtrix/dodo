using SimpleHttpServer;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;

namespace XR.Dodo
{
    static class SMSReader
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
            session.Messages.Add(new UserSession.Message() {
                MessageID = request.QueryParams["message_id"],
                DeviceID = request.QueryParams["device_id"],
                ReceiverNumber = request.QueryParams["sent_to"],
                MessageString = request.QueryParams["message"],
                TimeReceived = DateTime.FromFileTimeUtc(long.Parse(request.QueryParams["sent_timestamp"])),
            });
            return new HttpResponse()
            {
                ContentAsUTF8 = "I have received your message!",
                ReasonPhrase = "OK",
                StatusCode = "200"
            };
        }
    }
}
