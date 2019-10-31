using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dodo.Dodo
{
	[WorkflowTaskInfo(EUserAccessLevel.Coordinator)]
	public class CoordinatorCheckin : WorkflowTask
	{
		public static string CommandKey { get { return "CHECKIN"; } }
		public static string HelpString { get { return $"{CommandKey} - tell us how you are doing."; } }

		const string StartString = "I know that being a Rebel can be tough. " +
					"I'd like to ask you a few questions about how you are feeling and coping with it all. " +
					"This can help the Rebellion Support Office know who needs help. Remember - we're all crew, and we all need to take care of each other. ";

		const string StressQuestion = "On a scale of 1-5, how stressed do you feel right now? 1 being calm and okay, and 5 being very stressed.";
		public int Stress = -1;

		const string UnderstaffedQuestion = "On a scale of 1-5, how understaffed do you currently feel? 1 meaning you have enough help right now, and 5 being desperate for people.";
		public int Understaffed = -1;

		const string UnsafeQuestion = "On a scale of 1-5, how safe do you currently feel? 1 meaning you feel completely safe, and 5 meaning you feel very unsafe.";
		public int Safety = -1;

		const string VolunteerCountQuestion = "Now, tell me how many volunteers in your Working Group are actively helping out right now. This can be a rough estimate.";
		public int VolunteerCount = -1;

		const string EverythingOkDoneString = "Okay, that's all my questions for now. Remember to check in every once in a while, and to take care of yourself.";

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var user = session.GetUser();
			
			var cmd = message.ContentUpper.FirstOrDefault();
			if (cmd == CommandKey)
			{
				response = new ServerMessage($"Thanks for checking in {user.Name ?? "Rebel"}. " + StartString + StressQuestion);
				return true;
			}

			if(Stress < 0)
			{
				if(int.TryParse(cmd, out var stressNumber) && stressNumber >= 1 && stressNumber <= 5)
				{
					Stress = stressNumber;
					response = new ServerMessage(UnderstaffedQuestion);
					return true;
				}
				else
				{
					response = new ServerMessage("Sorry, I didn't understand that. If you want to stop checking-in, reply DONE. " + StressQuestion);
					return true;
				}
			}

			if (Understaffed < 0)
			{
				if (int.TryParse(cmd, out var understaffedNumber) && understaffedNumber >= 1 && understaffedNumber <= 5)
				{
					Understaffed = understaffedNumber;
					response = new ServerMessage(UnsafeQuestion);
					return true;
				}
				else
				{
					response = new ServerMessage("Sorry, I didn't understand that. If you want to stop checking-in, reply DONE. " + UnderstaffedQuestion);
					return true;
				}
			}

			if (Safety < 0)
			{
				if (int.TryParse(cmd, out var safeNumber) && safeNumber >= 1 && safeNumber <= 5)
				{
					Safety = safeNumber;
					response = new ServerMessage(VolunteerCountQuestion);
					return true;
				}
				else
				{
					response = new ServerMessage("Sorry, I didn't understand that. If you want to stop checking-in, reply DONE. " + UnsafeQuestion);
					return true;
				}
			}

			if(VolunteerCount < 0)
			{
				if (int.TryParse(cmd, out var safeNumber))
				{
					VolunteerCount = safeNumber;
					response = new ServerMessage(EverythingOkDoneString);
					ExitTask(session);
					DodoServer.CoordinatorNeedsManager.AddCheckin(user, Stress, Understaffed, Safety, VolunteerCount);
					return true;
				}
				else
				{
					response = new ServerMessage("Sorry, I didn't understand that. If you want to stop checking-in, reply DONE. Reply with a specific number, even if it's an estimate. " + VolunteerCountQuestion);
					return true;
				}
			}

			response = default;
			return true;
		}
	}
}
