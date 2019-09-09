using System.Collections.Generic;

namespace XR.Dodo
{
	public class VolunteerSession : UserSession
	{
		private VolunteerWorkflow m_workflow;
		public VolunteerSession(string user) : base(user)
		{
			m_workflow = new VolunteerWorkflow();
		}
		public override Workflow Workflow { get { return m_workflow; } }
	}
}
