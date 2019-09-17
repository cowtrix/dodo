using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public class CoordinatorWorkflow : Workflow
	{
		protected override ServerMessage GetHelp()
		{
			return new ServerMessage("TODO");
		}

		protected override ServerMessage ProcessMessageForRole(UserMessage message, UserSession session)
		{
			var toUpper = message.Content.ToUpperInvariant();
			if(toUpper.StartsWith(CoordinatorNeedsTask.CommandKey))
			{
				CurrentTask = new CoordinatorNeedsTask(this);
				return CurrentTask.ProcessMessage(message, session);
			}
			if (toUpper.StartsWith(CoordinatorWhoIsTask.CommandKey))
			{
				CurrentTask = new CoordinatorWhoIsTask(this);
				return CurrentTask.ProcessMessage(message, session);
			}
			if (toUpper.StartsWith(CoordinatorRemoveNeedTask.CommandKey))
			{
				CurrentTask = new CoordinatorRemoveNeedTask(this);
				return CurrentTask.ProcessMessage(message, session);
			}
			return GetHelp();
		}
	}
}
