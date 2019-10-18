using System;
using System.Collections.Generic;
using Common;
using Newtonsoft.Json;

namespace XR.Dodo
{
	public class UserSession
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

		public Workflow Workflow = new Workflow();
		public ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			try
			{
				var msg = Workflow.ProcessMessage(message, session);
				msg.TimeStamp = DateTime.Now;
				session.Outbox.Add(msg);
				return msg;
			}
			catch(Exception e)
			{
				Logger.Exception(e, "Unhandled exception in workflow");
				return new ServerMessage("Sorry, something went wrong. Please try again.");
			}
		}
	}
}
