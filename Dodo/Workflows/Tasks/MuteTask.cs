using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XR.Dodo
{
	[WorkflowTaskInfo(EUserAccessLevel.Volunteer)]
	public class MuteTask : WorkflowTask
	{
		public static string CommandKey { get { return "MUTE"; } }
		public static string HelpString { get { return $"{CommandKey} - stop me talking to you until you message me again"; } }

		public string WorkingGroup;
		public string RoleName;

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var user = session.GetUser();
			user.Active = false;
			response = new ServerMessage("Okay, I won't bother you until you message me again.");
			return true;
		}
	}
}
