using System;
using System.Collections.Generic;

namespace XR.Dodo
{

	public abstract class Workflow
	{
		public static TimeSpan m_timeout { get { return TimeSpan.FromMinutes(5); } }
		public WorkflowTask CurrentTask;
		public ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			session.Inbox.Add(message);
			var user = session.GetUser();			
			if(CurrentTask != null)
			{
				if(DateTime.Now - CurrentTask.TimeCreated > m_timeout)
				{
					CurrentTask.ExitTask();
				}
				else
				{
					return CurrentTask.ProcessMessage(message, session);
				}
			}
			switch (message.Content.ToUpperInvariant())
			{
				case "HELP":
					return GetHelp();
				case "VERIFY":
					CurrentTask = new Verification(this);
					return CurrentTask.ProcessMessage(message, session);
			}
			var response = ProcessMessageForRole(message, session);
			session.Outbox.Add(response);
			return response;
		}
		protected abstract ServerMessage ProcessMessageForRole(UserMessage message, UserSession session);
		protected abstract ServerMessage GetHelp();
	}
}
