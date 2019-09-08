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
				if(message.Content.ToLowerInvariant() == "inducted")
				{
					State = EVolunteerState.Inducted;
				}
			}
			DodoServer.TelegramGateway.SendMessage(session.GetUser(), new ServerMessage("Kapow!"));
			return new ServerMessage("Sorry, I didn't understand that.");
		}
	}
}
