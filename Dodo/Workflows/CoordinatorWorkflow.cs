using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public class CoordinatorWorkflow : Workflow
	{
		public CoordinatorWorkflow()
		{
			AddTask<CoordinatorWhoIsTask>();
			AddTask<CoordinatorNeedsTask>();
			AddTask<CoordinatorRemoveNeedTask>();
		}

		protected override bool ProcessMessageForRole(UserMessage message, UserSession session, out ServerMessage response)
		{
			response = default;
			return false;
		}
	}
}
