using System.Collections.Generic;

namespace XR.Dodo
{
	public class CoordinatorSession : UserSession
	{
		public CoordinatorSession(string user) : base(user)
		{
			m_workflow = new CoordinatorWorkflow();
		}
		private CoordinatorWorkflow m_workflow;
		public override Workflow Workflow { get { return m_workflow; } }
	}
}
