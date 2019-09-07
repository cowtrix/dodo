// Copyright (C) 2016 by Barend Erasmus, David Jeske and donated to the public domain

using System.Collections.Generic;

namespace XR.Dodo
{
	public class VolunteerWorkflow : Workflow
	{
		protected override IEnumerable<Message> ProcessMessageInternal(Message message, UserSession session)
		{
			return new[]
			{
				new Message(DodoServer.Server, "Thank you for registering!"),
			};
		}
	}
}
