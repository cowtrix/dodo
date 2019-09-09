using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace XR.Dodo
{
	public abstract class UserSession
	{
		public string UserID;
		public List<UserMessage> Inbox = new List<UserMessage>();
		public List<ServerMessage> Outbox = new List<ServerMessage>();

		public UserSession(string uuid)
		{
			UserID = uuid;
		}

		public User GetUser()
		{
			return DodoServer.SessionManager.GetUserFromUserID(UserID);
		}

		public abstract Workflow Workflow { get; }

		public ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			return Workflow.ProcessMessage(message, session);
		}
	}
}
