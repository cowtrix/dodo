using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace XR.Dodo
{
	public class UserSession
	{
		public User User;
		public List<Message> Messages = new List<Message>();
		public List<Message> Responses = new List<Message>();
		public UserSession(User user)
		{
			User = user;
		}
	}
}
