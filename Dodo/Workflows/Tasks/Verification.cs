using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public class VerificationTask : WorkflowTask
	{
		public static string CommandKey { get { return "VERIFY"; } }
		public static string HelpString { get { return $"{CommandKey} - use this to verify your phone number. A verified phone number is necessary for asking me to do some things."; } }

		public VerificationTask(Workflow workflow) : base(workflow)
		{
		}

		private string GetVerificationNumber()
		{
			return DodoServer.SMSGateway.GetPhone(Phone.ESMSMode.Verification).Number;
		}

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			if (message.Gateway.Type != EGatewayType.Telegram)
			{
				response = new ServerMessage("Sorry, you need to message me on Telegram to verify your number. " +
					$"You can do this by messaging VERIFY to {DodoServer.TelegramGateway.UserName}.");
				return true;
			}
			var user = session.GetUser();
			if (user.IsVerified())
			{
				session.Verification = null;
				response = new ServerMessage("It looks like you're already verified for " + session.GetUser().PhoneNumber);
				return true;
			}
			if (session.Verification == null || (DateTime.Now - session.Verification.TimeSent) > Timeout)
			{
				session.Verification = new UserSession.VerificationState()
				{
					Code = new Random().Next(10000, 99999).ToString(),
					TimeSent = DateTime.Now,
				};
				response = new ServerMessage("Please take a moment to verify your phone number. " +
					$"You can do this by texting {session.Verification.Code} to {GetVerificationNumber()}." +
					$"This code will expire in {Timeout.TotalMinutes} minutes.");
				ExitTask();
				return true;
			}
			response = new ServerMessage($"I'm still waiting for your verification code. SMS {session.Verification.Code} to {GetVerificationNumber()}." +
					$"This code will expire in {Timeout.TotalMinutes} minutes.");
			ExitTask();
			return true;
		}
	}
}
