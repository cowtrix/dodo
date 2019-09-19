using System.Collections.Generic;

namespace XR.Dodo
{
	public class VolunteerWorkflow : Workflow
	{
		protected override bool ProcessMessageForRole(UserMessage message, UserSession session, out ServerMessage response)
		{
			response = default;
			return false;
		}
	}
}
