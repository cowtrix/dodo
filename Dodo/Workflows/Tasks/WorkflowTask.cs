using Newtonsoft.Json;
using System;

namespace XR.Dodo
{
	public abstract class WorkflowTask
	{
		public DateTime TimeCreated;

		[JsonIgnore]
		protected readonly Workflow m_workflow;
		[JsonIgnore]
		public virtual TimeSpan Timeout { get { return TimeSpan.FromMinutes(5); } }

		public WorkflowTask(Workflow workflow)
		{
			m_workflow = workflow;
			TimeCreated = DateTime.Now;
		}

		public abstract bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response);

		public virtual ServerMessage ExitTask()
		{
			m_workflow.CurrentTask = null;
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
