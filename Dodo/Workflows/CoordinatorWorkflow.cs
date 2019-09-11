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
			if(toUpper.StartsWith("NEED"))
			{
				CurrentTask = new CoordinatorNeedsTask(this);
				return CurrentTask.ProcessMessage(message, session);
			}
			return GetHelp();
		}
	}
}
