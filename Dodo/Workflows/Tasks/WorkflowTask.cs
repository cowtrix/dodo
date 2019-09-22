using Newtonsoft.Json;
using System;

namespace XR.Dodo
{
	public abstract class WorkflowTask
	{
		public DateTime TimeCreated;

		[JsonIgnore]
		public virtual TimeSpan Timeout { get { return TimeSpan.FromMinutes(5); } }

		public abstract bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response);

		public virtual ServerMessage ExitTask(UserSession session)
		{
			session.Workflow.CurrentTask = null;
			return new ServerMessage("Okay, I've canceled that.");
		}

		public virtual bool GetHelp(out ServerMessage response)
		{
			response = default;
			return false;
		}

		public virtual bool CanCancel()
		{
			return true;
		}
	}
}
