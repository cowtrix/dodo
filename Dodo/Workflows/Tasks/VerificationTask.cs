using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XR.Dodo
{
	[WorkflowTaskInfo(EUserAccessLevel.Volunteer)]
	public class VerificationTask : WorkflowTask
	{
		public static string CommandKey { get { return "VERIFY"; } }
		public static string HelpString { get { return $"{CommandKey} - verify your phone number with us."; } }

		public static bool IsValid(User user)
		{
			return !user.IsVerified();
		}

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var user = session.GetUser();
			if(user.IsVerified())
			{
				response = new ServerMessage("It looks like you've already verified your number as " + user.PhoneNumber);
				ExitTask(session);
				return true;
			}
			DodoServer.TelegramGateway.SendNumberRequest("Please take a moment to share your phone number with us by pressing the button on your keyboard below.", 
				session);
			response = default;
			return true;
		}
	}
}
