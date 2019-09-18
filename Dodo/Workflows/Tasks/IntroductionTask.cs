using System;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	public class IntroductionTask : WorkflowTask
	{
		public IntroductionTask(Workflow workflow) : base(workflow)
		{
		}

		const string VolunteerAgreement = "https://actionnetwork.org/forms/xr-data-protection-agreement-2";

		public override TimeSpan Timeout { get { return TimeSpan.MaxValue; } }

		enum EState
		{
			VolunteerAgreement,
			GetName,
			GetStartingDate,
			GetEndDate,
			Tutorial,
		}
		EState m_state = EState.VolunteerAgreement;

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var user = session.GetUser();
			if (!user.GDPR)
			{
				if(session.Inbox.Count == 1)    // First ever message
				{
					response = new ServerMessage("Hello! I'm your friendly Extinction Rebellion Bot. Whether you're a seasoned rebel, or you're just getting started, " +
						"my purpose is to help you get involved in the Autumn Rebellion in London. " +
						$"Firstly, take a second to read and sign our Volunteer Agreement here: {VolunteerAgreement}\n" +
						"When you've done that, reply with the word DONE, and we can move onto the next step.");
					return true;
				}
				if (message.ContentUpper.FirstOrDefault() == "DONE")
				{
					m_state = EState.GetName;
					response = new ServerMessage("Fantastic! Now, I'd like to find out a bit more about you. What's your name? You can give me your real name, or a nickname, I don't mind.");
					user.GDPR = true;
					return true;
				}
				else
				{
					response = new ServerMessage("Sorry, I didn't understand that. If you've signed the volunteer agreement here: "
						+ VolunteerAgreement + " then just reply with the word DONE to continue.");
					return true;
				}
			}

			if(m_state == EState.GetName)
			{
				user.Name = message.Content.Substring(0, Math.Min(message.Content.Length, 64)).Trim();
				response = new ServerMessage($"Hello {user.Name}! Now, if you already know how you're getting involved in the Autumn Rebellion, " +
					"you can just reply CANCEL. Otherwise, you can tell me a little more about yourself. Firstly, what date will you be arriving to the rebellion? " +
					"For instance, if you were arriving on the 7th of October, you would reply 7/10");
				m_state = EState.GetStartingDate;
				return true;
			}

			if(m_state == EState.GetStartingDate)
			{
				var split = message.ContentUpper.FirstOrDefault().Split('/');
				DateTime startDate;
				try
				{
					if (split.Length != 2 || !int.TryParse(split[0], out var day) || !int.TryParse(split[1], out var month))
					{
						throw new Exception();
					}
					startDate = new DateTime(2019, month, day);
				}
				catch
				{
					response = new ServerMessage($"Sorry, I didn't quite understand that date. For instance, if you were arriving on the 12th of October, you would reply 12/10" +
						" Or if you want to stop telling me about yourself, reply CANCEL.");
					return true;
				}
				if(startDate < new DateTime(2019, 10, 7))
				{
					response = new ServerMessage($"Looks like you're an early bird! But the rebellion starts on the 7th of October, so that's the earliest you can arrive. If that's the case, reply 7/10." +
						" Or if you want to stop telling me about yourself, reply CANCEL.");
					return true;
				}
				user.StartDate = startDate;
				response = new ServerMessage($"Amazing! I'll see you on {user.StartDate.ToShortDateString()}. Now, do you know when you'll be leaving?" +
					" Reply again with a date, or if you don't know, just say NO");
				m_state = EState.GetEndDate;
				return true;
			}

			if (m_state == EState.GetEndDate)
			{
				if (message.ContentUpper.FirstOrDefault() != "NO")
				{
					var split = message.ContentUpper.FirstOrDefault().Split('/');
					DateTime endDate;
					try
					{
						if (split.Length != 2 || !int.TryParse(split[0], out var day) || !int.TryParse(split[1], out var month))
						{
							throw new Exception();
						}
						endDate = new DateTime(2019, month, day);
					}
					catch
					{
						response = new ServerMessage($"Sorry, I didn't quite understand that date. For instance, if you were arriving on the 20th of October, you would reply 20/10" +
							" Or if you aren't sure, just say NO, or if you want to stop telling me about yourself, reply CANCEL.");
						return true;
					}
					if (endDate < user.StartDate)
					{
						response = new ServerMessage($"Hmm, {endDate.ToShortDateString()} can't be right, it sounds like you're leaving before you arrive. " +
							" If you aren't sure, just say NO, or if you want to stop telling me about yourself, reply CANCEL.");
						return true;
					}
					user.EndDate = endDate;
				}
				m_state = EState.Tutorial;
			}

			if (message.Gateway.Type == EGatewayType.Telegram)
			{
				response = new ServerMessage($"Brilliant, thanks {user.Name}. The next step is to verify your phone number so we can stay in touch with you. To do this, reply VERIFY.");
				return true;
			}

			response = default;
			return false;
			/*
			$"You can talk to me on Telegram at {DodoServer.TelegramGateway.UserName}, or you can SMS me at {DodoServer.GetSMSNumber()}. " +
			"I like Telegram more though, and it means I can be a bit more helpful to you. " +*/
		}

		public override bool CanCancel()
		{
			return m_state > EState.VolunteerAgreement;
		}
	}
}
