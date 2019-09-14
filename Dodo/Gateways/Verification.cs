using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public class Verification : WorkflowTask
	{
		public string CodeString;
		public bool SentPromptString;
		public DateTime GenerationDate;

		public Verification(Workflow workflow) : base(workflow)
		{
		}

		public override ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			var cmd = message.Content.ToUpperInvariant();
			if (!SentPromptString)
			{
				SentPromptString = true;
				CodeString = new Random().Next(10000, 99999).ToString();
				GenerationDate = DateTime.Now;
				return new ServerMessage("Please take a moment to verify your phone number. " + 
					$"You can do this by texting {CodeString} to {DodoServer.GetSMSNumber()}." + 
					"This code will expire in 5 minutes.");
			}
			if(message.GatewayType != EGatewayType.SMS)
			{
				if (cmd == "VERIFY")
				{
					return new ServerMessage("Please take a moment to verify your phone number." +
						$"You can do this by texting {CodeString} to {DodoServer.GetSMSNumber()}");
				}
				return default(ServerMessage);
			}
			if (message.Content == CodeString)
			{
				session.GetUser().PhoneNumber = message.Source;
				return default;
			}
			return new ServerMessage("Sorry, that was the wrong code. You can try again, or if you'd like to skip verification for now, reply SKIP.");
		}

		/*public override ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			if (message.Content.ToUpperInvariant() == "CANCEL")
			{
				Skip = true;
			}
			if(!SentPromptString)
			{
				SentPromptString = true;
				return new ServerMessage("Please take a moment to tell me your phone number.");
			}
			if(string.IsNullOrEmpty(CandidateNumber))
			{
				var number = message.Content;
				if (!ValidationExtensions.ValidateNumber(ref number))
				{
					return new ServerMessage("Sorry, that doesn't seem like a valid UK mobile phone number to me." +
						" You can try again, or if you'd like to skip verification for now, reply SKIP.");
				}
				CandidateNumber = number;
				CodeString = new Random().Next(10000, 99999).ToString();
				GenerationDate = DateTime.Now;
				var twilio = new ServerMessage($"Your XR Verification code: {CodeString}");
				DodoServer.SendSMS(twilio, CandidateNumber);
				return new ServerMessage($"Great, I've sent a verification code to {number}. When you get it, send it to me here.");
			}
			if(message.Content == CodeString)
			{
				session.GetUser().PhoneNumber = CandidateNumber;
				return default;
			}
			return new ServerMessage("Sorry, that was the wrong code. You can try again, or if you'd like to skip verification for now, reply SKIP.");			
		}*/
	}
}
