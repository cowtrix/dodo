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
				if((taskType.Value == typeof(RolesTask) || taskType.Value == typeof(SettingsTask)) && user.AccessLevel > EUserAccessLevel.Volunteer)
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
				sb.AppendLine("Or for more info, check out the User Guide here: " + DodoServer.VolunteerUserGuideURL + 
					" When a role comes up that I think you might be interested in, I'll send you a message about it. " +
					"That message will contain a code that you can respond with, which will tell me that you're interested. " + 
					"Then, I'll share your number with a coordinator who will be in touch about the role. ");
			}
			sb.AppendLine($"If you still need help, you can email {DodoServer.SupportEmail} for more assistance.");
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
