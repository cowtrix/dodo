using System.Linq;

namespace XR.Dodo
{
	[WorkflowTaskInfo(EUserAccessLevel.Volunteer)]
	public class SettingsTask : WorkflowTask
	{
		public static string CommandKey { get { return "SETTINGS"; } }
		public static string HelpString { get { return $"{CommandKey} - change how the Bot interacts with you."; } }

		public const string StartString = "Hi Rebel - what a start to the Rebellion it's been! There's been a lot of roles that need filling, but I don't want to spam you too much. If you'd like, you can tell me to message you less by replying LESS. If you are ok with the amount of messages you have been receiving, reply DONE. Or, if you want more role requests sent to you, reply MORE";
		const string Followup = "Sorry, I didn't understand that. ";
		const string Instructions = "You can tell me to message you less by replying LESS. If you are ok with the amount of messages you have been receiving, reply DONE. Or, if you want more role requests sent to you, reply MORE";
		string Finish { get { return $"Okay, I'll remember that. You can update this any time with the {CommandKey} command"; } }
		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var user = session.GetUser();
			var cmd = message.ContentUpper.FirstOrDefault();
			if(cmd == CommandKey)
			{
				response = new ServerMessage(Instructions);
				return true;
			}
			if(cmd == "LESS")
			{
				user.SpamSetting = User.ESpamSetting.LESS;
				response = new ServerMessage(Finish);
				ExitTask(session);
				return true;
			}
			else if (cmd == "MORE")
			{
				user.SpamSetting = User.ESpamSetting.MORE;
				response = new ServerMessage(Finish);
				ExitTask(session);
				return true;
			}
			else
			{
				response = new ServerMessage(Followup + Instructions);
				return true;
			}
		}
	}
}
