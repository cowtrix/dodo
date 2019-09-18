using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public class Verification : WorkflowTask
	{
		public static string CommandKey { get { return "VERIFY"; } }

		public string CodeString;
		public bool SentPromptString;
		public DateTime GenerationDate;

		public Verification(Workflow workflow) : base(workflow)
		{
		}

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			if(session.GetUser().IsVerified())
			{
				response = new ServerMessage("It looks like you're already verified.");
			}
			var cmd = message.Content.ToUpperInvariant();
			if (!SentPromptString)
			{
				SentPromptString = true;
				CodeString = new Random().Next(10000, 99999).ToString();
				GenerationDate = DateTime.Now;
				response = new ServerMessage("Please take a moment to verify your phone number. " +
					$"You can do this by texting {CodeString} to {DodoServer.GetSMSNumber()}." +
					"This code will expire in 5 minutes.");
				return true;
			}
			if(message.Gateway.Type == EGatewayType.Telegram)
			{
				if (cmd == "VERIFY")
				{
					response = new ServerMessage("Please take a moment to verify your phone number." +
						$"You can do this by texting {CodeString} to {DodoServer.GetSMSNumber()}");
				}
				response = default;
				return false;
			}
			if (message.ContentUpper.FirstOrDefault() == CodeString)
			{
				var ph = message.Source;
				if(ValidationExtensions.ValidateNumber(ref ph))
				{
					session.GetUser().PhoneNumber = ph;
					DodoServer.TelegramGateway.SendMessage(new ServerMessage("Awesome, you've verified your phone number as " + ph), session);
					ExitTask();
				}
				response = default;
				return true;
			}
			response = new ServerMessage("Sorry, that was the wrong code. You can try again, or if you'd like to skip verification for now, reply SKIP.");
			return true;
		}
	}
}
