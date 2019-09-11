using System;

namespace XR.Dodo
{
	public abstract class WorkflowTask
	{
		[NonSerialized]
		protected readonly Workflow m_workflow;
		public DateTime TimeCreated;
		public WorkflowTask(Workflow workflow)
		{
			m_workflow = workflow;
			TimeCreated = DateTime.Now;
		}

		public abstract ServerMessage ProcessMessage(UserMessage message, UserSession session);

		public virtual void ExitTask()
		{
			m_workflow.CurrentTask = null;
		}
	}
}
