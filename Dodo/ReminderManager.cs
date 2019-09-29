using Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public class ReminderManager
	{
		public ReminderManager()
		{
			var introReminderTask = new Task(async () =>
			{
				while (true)
				{
					var sm = DodoServer.SessionManager;
					try
					{
						var now = DateTime.Now;
						if (now.Hour < 24 && now.Hour > 8)
						{
							var introUsers = sm.GetUsers()
								.Where(user =>
								{
									var session = sm.GetOrCreateSession(user);
									var introTask = session.Workflow.CurrentTask as IntroductionTask;
									return user.TelegramUser > 0 && introTask != null && !introTask.ReminderSent && DateTime.Now - session.Inbox.LastOrDefault().TimeStamp > TimeSpan.FromHours(1);
								});
							foreach (var user in introUsers)
							{
								var session = sm.GetOrCreateSession(user);
								var introTask = session.Workflow.CurrentTask as IntroductionTask;
								if (introTask == null)
								{
									continue;
								}
								introTask.ReminderSent = true;
								if (!user.GDPR)
									DodoServer.TelegramGateway.SendMessage(new ServerMessage($"Hi Rebel! Are you ready to get started? When you've signed the October Volunteer Agreement, reply DONE. "), session);
								else if (!user.IsVerified())
									DodoServer.TelegramGateway.SendNumberRequest("Hello Rebel! I'm still waiting for you to verify your number. To do that, just press the button below that appears where your keyboard normally is - the one that says PRESS HERE TO CONTINUE", session);
								else
								{
									DodoServer.TelegramGateway.SendMessage(new ServerMessage("Hello Rebel! I'm still waiting for you to answer some questions, and then I can contact you about roles. If you're done introducing yourself to me, just reply DONE. You can update any of this information later by replying INFO"), session);
								}
							}
						}
					}
					catch(Exception e)
					{
						Logger.Exception(e);
					}
					await Task.Delay(TimeSpan.FromMinutes(10));
				}
			});
			introReminderTask.Start();
		}
	}
}
