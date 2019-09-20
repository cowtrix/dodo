using System;
using System.Reflection;
using System.Text;

namespace XR.Dodo
{
	public class HelpTask : WorkflowTask
	{
		public static string CommandKey { get { return "HELP"; } }
		public static string HelpString { get { return $"{CommandKey} - use this to ask for help, if you're not sure what to do."; } }

		public HelpTask(Workflow workflow) : base(workflow)
		{
		}

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var sb = new StringBuilder("You can tell me to do things by sending me commands, which are simple words. Here's a list of what you can do right now:\n");
			sb.AppendLine();
			foreach(var taskType in m_workflow.Tasks)
			{
				var helpStr = GetHelpStringFromType(taskType.Value);
				if(string.IsNullOrEmpty(helpStr))
				{
					continue;
				}
				sb.AppendLine(helpStr);
			}
			sb.AppendLine();
			/*sb.AppendLine("If you want help to use a specific command, just say the command name, and then HELP - for instance, INFO HELP." +
				" If you're still confused, reply HELP CONTACT and someone will be in touch to help you out.");*/
			response = new ServerMessage(sb.ToString());
			ExitTask();
			return true;
		}

		string GetHelpStringFromType(Type t)
		{
			return t.GetProperty("HelpString", BindingFlags.Public | BindingFlags.Static).GetValue(null) as string;
		}
	}
}
