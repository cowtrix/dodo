using System;
using System.Collections.Generic;

namespace XR.Dodo
{

	public class UserSession
	{
		public string Number;
		public List<Message> Messages = new List<Message>();
		public List<MessageResponse> Responses = new List<MessageResponse>();

		public UserSession(string fromNumber)
		{
			Number = fromNumber;
		}
	}
}
