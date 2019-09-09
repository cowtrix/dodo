using System;
using System.Collections.Generic;

namespace XR.Dodo
{
	public abstract class Workflow
	{
		public Verification Verification = new Verification();
		public ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			session.Inbox.Add(message);
			var user = session.GetUser();
			var response = ProcessMessageInternal(message, session);
			session.Outbox.Add(response);
			return response;
		}

		protected abstract ServerMessage ProcessMessageInternal(UserMessage message, UserSession session);
	}
}
