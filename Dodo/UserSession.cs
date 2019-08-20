using System;
using System.Collections.Generic;

namespace XR.Dodo
{
    public class UserSession
    {
        public struct Message
        {
            public string MessageString;
            public DateTime TimeReceived;
            public string MessageID;
            public string ReceiverNumber;
            public string DeviceID;
        }
        public string Number;
        public List<Message> Messages = new List<Message>();

        public UserSession(string fromNumber)
        {
            Number = fromNumber;
        }
    }
}
