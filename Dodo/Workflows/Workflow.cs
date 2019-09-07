// Copyright (C) 2016 by Barend Erasmus, David Jeske and donated to the public domain

using System;
using System.Collections.Generic;

namespace XR.Dodo
{
	public abstract class Workflow
	{
		public IEnumerable<Message> ProcessMessage(Message message, UserSession session)
		{
			return ProcessMessageInternal(message, session);
		}

		protected abstract IEnumerable<Message> ProcessMessageInternal(Message message, UserSession session);
	}
}
