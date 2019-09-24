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
			foreach(var taskType in session.Workflow.Tasks)
			{
				if(GetMinimumAccessLevel(taskType.Value) > session.GetUser().AccessLevel)
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
