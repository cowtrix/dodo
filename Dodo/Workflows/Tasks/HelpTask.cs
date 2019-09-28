using System;
using System.Reflection;
using System.Text;

namespace XR.Dodo
{
	[WorkflowTaskInfo(EUserAccessLevel.Volunteer)]
	public class HelpTask : WorkflowTask
	{
		public static string CommandKey { get { return "HELPME"; } }
		public static string HelpString { get { return $"{CommandKey} - ask for help, if you're not sure what to do."; } }

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var sb = new StringBuilder("You can tell me to do things by sending me commands, which are simple words. Here's a list of what you can do right now:\n");
			sb.AppendLine();
			var user = session.GetUser();
			foreach (var taskType in session.Workflow.Tasks)
			{
				if(GetMinimumAccessLevel(taskType.Value) > user.AccessLevel)
				{
					continue;
				}
				if(taskType.Value == typeof(RolesTask) && user.AccessLevel > EUserAccessLevel.Volunteer)
				{
					continue;
				}
				if(taskType.Value == typeof(VerificationTask) && (session.GetUser().IsVerified() || message.Gateway.Type != EGatewayType.Telegram))
				{
					continue;
				}
				var helpStr = GetHelpStringFromType(taskType.Value);
				if(string.IsNullOrEmpty(helpStr))
				{
					continue;
				}
				sb.AppendLine(helpStr);
			}
			sb.AppendLine();
			if (user.AccessLevel > EUserAccessLevel.Volunteer)
			{
				sb.AppendLine("Or for more info, check out the Coordinator User Guide here: " + DodoServer.CoordinatorUserGuideURL);
			}
			else
			{
				sb.AppendLine("Or for more info, check out the User Guide here: " + DodoServer.VolunteerUserGuideURL);
			}
			
			response = new ServerMessage(sb.ToString());
			ExitTask(session);
			return true;
		}

		string GetHelpStringFromType(Type t)
		{
			return t.GetProperty("HelpString", BindingFlags.Public | BindingFlags.Static).GetValue(null) as string;
		}
	}
}
