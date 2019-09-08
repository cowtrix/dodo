// Copyright (C) 2016 by Barend Erasmus, David Jeske and donated to the public domain

using System;
using System.Collections.Generic;

namespace XR.Dodo
{
	public abstract class Workflow
	{
		public ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			session.Inbox.Add(message);
			var response = ProcessMessageInternal(message, session);
			session.Outbox.Add(response);
			return response;
		}

		protected abstract ServerMessage ProcessMessageInternal(UserMessage message, UserSession session);
	}
}
