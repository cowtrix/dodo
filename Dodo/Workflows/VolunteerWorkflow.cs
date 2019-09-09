using System.Collections.Generic;

namespace XR.Dodo
{
	public class VolunteerWorkflow : Workflow
	{
		public enum EVolunteerState
		{
			New,
			Inducted,
		}
		public EVolunteerState State;
		protected override ServerMessage ProcessMessageInternal(UserMessage message, UserSession session)
		{
			if(State == EVolunteerState.New)
			{
				if(message.Content.ToUpperInvariant() == "INDUCTED")
				{
					State = EVolunteerState.Inducted;
				}
			}
			return new ServerMessage("Sorry, I didn't understand that.");
		}
	}
}
