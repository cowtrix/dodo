using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public class Verification
	{
		struct Code
		{
			public string CodeString;
			public string CandidateNumber;
			public bool SentPromptString;
			public DateTime GenerationDate;
		}
		Code CurrentCode;
		public bool Skip;
		public async Task<ServerMessage?> Verify(UserMessage message, UserSession session)
		{
			if(message.Content.ToUpperInvariant() == "SKIP")
			{
				Skip = true;
			}
			if(!CurrentCode.SentPromptString)
			{
				CurrentCode.SentPromptString = true;
				return new ServerMessage("Please take a moment to tell me your phone number.");
			}
			if(string.IsNullOrEmpty(CurrentCode.CandidateNumber))
			{
				var number = message.Content;
				if (!ValidationExtensions.ValidateNumber(ref number))
				{
					return new ServerMessage("Sorry, that doesn't seem like a valid UK mobile phone number to me." +
						" You can try again, or if you'd like to skip verification for now, reply SKIP.");
				}
				CurrentCode.CandidateNumber = number;
				CurrentCode.CodeString = new Random().Next(10000, 99999).ToString();
				CurrentCode.GenerationDate = DateTime.Now;
				var twilio = new ServerMessage($"Your XR Verification code: {CurrentCode.CodeString}");
				DodoServer.SendSMS(twilio, CurrentCode.CandidateNumber);
				return new ServerMessage($"Great, I've sent a verification code to {number}. When you get it, send it to me here.");
			}
			if(message.Content == CurrentCode.CodeString)
			{
				session.GetUser().PhoneNumber = CurrentCode.CandidateNumber;
				return null;
			}
			return new ServerMessage("Sorry, that was the wrong code. You can try again, or if you'd like to skip verification for now, reply SKIP.");			
		}
	}
}
