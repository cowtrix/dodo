using System.Collections.Generic;

namespace XR.Dodo
{
	public class VolunteerWorkflow : Workflow
	{
		/*public enum EVolunteerState
		{
			New,
			Inducted,
		}
		public EVolunteerState State;

		protected override bool ProcessMessageForRole(UserMessage message, UserSession session)
		{
			if(State == EVolunteerState.New)
			{
				if(message.Content.ToUpperInvariant() == "INDUCTED")
				{
					State = EVolunteerState.Inducted;
				}
			}
			return GetHelp();
		}

		protected override ServerMessage GetHelp()
		{
			return new ServerMessage("Sorry, I didn't understand that. If you need some help as to what I can do, reply HELP");
		}*/
		protected override bool ProcessMessageForRole(UserMessage message, UserSession session, out ServerMessage response)
		{
			response = default;
			return false;
		}
	}
}
