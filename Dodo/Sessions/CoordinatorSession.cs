using System.Collections.Generic;

namespace XR.Dodo
{
	public class CoordinatorSession : UserSession
	{
		public CoordinatorSession(Coordinator user) : base(user)
		{
			m_workflow = new CoordinatorWorkflow();
		}
		private CoordinatorWorkflow m_workflow;
		protected override Workflow Workflow { get { return m_workflow; } }
	}
}
