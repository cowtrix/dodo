﻿using System;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	[WorkflowTaskInfo(EUserAccessLevel.Volunteer)]
	public class IntroductionTask : WorkflowTask
	{
		const string VolunteerAgreement = "https://actionnetwork.org/forms/xr-data-protection-agreement-2";

		public override TimeSpan Timeout { get { return TimeSpan.MaxValue; } }

		public EParentGroup? m_parentGroupFilter;

		public enum EState
		{
			VolunteerAgreement,
			TwilioVerify,
			GetName,
			GetSite,
			GetPrefs,
			GetStartingDate,
			GetEndDate,
			Tutorial,
		}
		public EState State = EState.VolunteerAgreement;

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var user = session.GetUser();
			if (!user.GDPR)
			{
				if(session.Inbox.Count == 1)	// First ever message
				{
					response = new ServerMessage("Hello! I'm your friendly Extinction Rebellion Bot. Whether you're a seasoned rebel, or you're just getting started, " +
						"my purpose is to help you get involved in the Autumn Rebellion in London. Through me, you can find out what needs doing, who needs help, and how you can get involved. " +
						$"Firstly, if you haven't yet, take a second to read and sign our Volunteer Agreement here: {VolunteerAgreement}\n" +
						"When you've done that, reply with the word DONE, and we can move onto the next step.");
					return true;
				}
				if (message.ContentUpper.FirstOrDefault() == "DONE")
				{
					user.GDPR = true;
					if (message.Gateway.Type != EGatewayType.Telegram)
					{
						State = EState.GetName;
						response = new ServerMessage("Fantastic! Now, I'd like to find out a bit more about you. What's your name? You can give me your real name, or a nickname, I don't mind.");
						return true;
					}
					State = EState.TwilioVerify;
				}
				else
				{
					response = new ServerMessage("Sorry, I didn't understand that. If you've signed the volunteer agreement here: "
						+ VolunteerAgreement + " then just reply with the word DONE to continue.");
					user.Karma--;
					return true;
				}
			}

			if(State == EState.TwilioVerify && message.Gateway.Type == EGatewayType.Telegram)
			{
				if(user.IsVerified())
				{
					if(user.AccessLevel > EUserAccessLevel.Volunteer)
					{
						ExitTask(session);
						response = new ServerMessage("Because you're a coordinator, I already have some information about you. Why not see what you can ask me to do by replying " + HelpTask.CommandKey);
						return true;
					}
					State = EState.GetName;
					response = new ServerMessage("Now, I'd like to find out a bit more about you. What's your name? You can give me your real name, or a nickname, I don't mind.");
					return true;
				}
				DodoServer.TelegramGateway.SendNumberRequest("Please share your phone number with me, so I can pass on your contact details to rebels looking for volunteers.", session);
				response = default;
				return true;
			}

			if(message.ContentUpper.FirstOrDefault() == "DONE")
			{
				response = new ServerMessage($"No problem. If you're ever confused, just reply {HelpTask.CommandKey} and I'll try to give you some guidance about what you can ask me to do. " +
					"Why don't you try that now?");
				ExitTask(session);
				return true;
			}

			if (State == EState.GetName)
			{
				var name = message.Content.Substring(0, Math.Min(message.Content.Length, 32)).Trim();
				user.Name = name;
				var nameString = $"Hello {user.Name}! ";
				if (user.Name != message.Content)
				{
					nameString = $"Well, that was a bit long, so how about {user.Name}. ";
				}
				nameString += "Now, if you already know how you're getting involved in the Autumn Rebellion, " +
						"you can just reply DONE. Otherwise, you can tell me a little more about yourself. Firstly, ";
				if (user.StartDate != default && DateTime.Now > user.StartDate)
				{
					response = new ServerMessage(nameString + "which site are you at? " + GetSiteList());
					return true;
				}
				response = new ServerMessage(nameString + "which site are you going to be at? " + GetSiteList());
				State = EState.GetSite;
				return true;
			}

			const string wgPrefsString = "Now you can tell me about what Working Groups you'd like to volunteer for. " +
				"You can find out about what each Working Group does here: http://www.REPLACE.ME\n";
			if (State == EState.GetSite)
			{
				if(message.ContentUpper.FirstOrDefault() == "SKIP")
				{
					response = new ServerMessage($"Okay, we can skip that. " + wgPrefsString + GetParentGroupSelectionString());
					State = EState.GetPrefs;
					return true;
				}
				if (!int.TryParse(message.ContentUpper.FirstOrDefault(), out var siteCode) || siteCode == 0 ||
					!DodoServer.SiteManager.IsValidSiteCode(siteCode))
				{
					response = new ServerMessage("Sorry, that didn't seem like a valid choice. " +
						GetSiteList());
					return true;
				}
				user.SiteCode = siteCode;
				if (user.StartDate != default && DateTime.Now > user.StartDate)
				{
					response = new ServerMessage($"Okay, you're at {DodoServer.SiteManager.GetSite(user.SiteCode).SiteName}. " +
						wgPrefsString + GetParentGroupSelectionString());
				}
				else
				{
					response = new ServerMessage($"Okay, you're going to be at the {DodoServer.SiteManager.GetSite(user.SiteCode).SiteName} site. " +
						wgPrefsString + GetParentGroupSelectionString());
				}
				State = EState.GetPrefs;
				return true;
			}

			var cmd = message.ContentUpper.FirstOrDefault();
			if (State == EState.GetPrefs)
			{
				const string responseString = "Now, what date will you be arriving to the rebellion? " +
						"For instance, if you were arriving on the 7th of October, you would reply 7/10";
				if (message.ContentUpper.FirstOrDefault() == "SKIP")
				{
					response = new ServerMessage($"Okay, we can skip that for now. " + responseString);
					State = EState.GetStartingDate;
					return true;
				}
				if (message.ContentUpper.FirstOrDefault() == "DONE")
				{
					response = new ServerMessage(responseString);
					State = EState.GetStartingDate;
					return true;
				}
				if (m_parentGroupFilter == null)
				{
					if(int.TryParse(cmd, out var pGroup) && pGroup >= 0 && pGroup <= (int)EParentGroup.RSO)
					{
						m_parentGroupFilter = (EParentGroup)pGroup;
						response = new ServerMessage(GetWorkingGroupList(user, m_parentGroupFilter.Value));
						return true;
					}
					// Get parent group
					response = new ServerMessage("Sorry, I didn't understand that selection. " + GetParentGroupSelectionString());
					return true;
				}
				else
				{
					if(cmd == "BACK")
					{
						response = new ServerMessage(GetParentGroupSelectionString());
						m_parentGroupFilter = null;
						return true;
					}
					if(DodoServer.SiteManager.IsValidWorkingGroup(cmd, out var workingGroup))
					{
						string addedOrRemoved;
						if(user.WorkingGroupPreferences.Contains(workingGroup.ShortCode))
						{
							addedOrRemoved = "Okay, I removed that.";
							user.WorkingGroupPreferences.Remove(workingGroup.ShortCode);
						}
						else
						{
							addedOrRemoved = "Okay, I added that.";
							user.WorkingGroupPreferences.Add(workingGroup.ShortCode);
						}
						response = new ServerMessage(addedOrRemoved + GetWorkingGroupList(user, m_parentGroupFilter.Value));
						return true;
					}
				}
			}

			if (State == EState.GetStartingDate)
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
					response = new ServerMessage($"Sorry, I didn't quite understand that date. For instance, if you were arriving on the 12th of October, you would reply 12/10." +
						" Or if you want to stop telling me about yourself, reply DONE.");
					user.Karma--;
					return true;
				}
				if(startDate < new DateTime(2019, 10, 7))
				{
					response = new ServerMessage($"Looks like you're an early bird! But the rebellion starts on the 7th of October, so that's the earliest you can arrive. If that's the case, reply 7/10." +
						" Or if you want to stop telling me about yourself, reply DONE.");
					return true;
				}
				user.StartDate = startDate;
				response = new ServerMessage($"Amazing! I'll see you on {user.StartDate.ToShortDateString()}. Now, do you know when you'll be leaving?" +
					" Reply again with a date, or if you don't know, just say NO");
				State = EState.GetEndDate;
				return true;
			}

			if (State == EState.GetEndDate)
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
							" Or if you aren't sure, just say NO, or if you want to stop telling me about yourself, reply DONE.");
						user.Karma--;
						return true;
					}
					if (endDate < user.StartDate)
					{
						response = new ServerMessage($"Hmm, {endDate.ToShortDateString()} can't be right, it sounds like you're leaving before you arrive. " +
							" If you aren't sure, just say NO, or if you want to stop telling me about yourself, reply DONE.");
						return true;
					}
					user.EndDate = endDate;
				}
				State = EState.Tutorial;
			}

			if (message.Gateway.Type == EGatewayType.Telegram)
			{
				response = new ServerMessage($"Brilliant, thanks {user.Name}. " +
					$"If you're ever confused, just reply {HelpTask.CommandKey} and I'll try to give you some guidance about what you can ask me to do. " +
					"Why don't you try that now?");
				ExitTask(session);
				return true;
			}
			ExitTask(session);
			response = default;
			return false;
			/*
			$"You can talk to me on Telegram at {DodoServer.DefaultGateway.UserName}, or you can SMS me at {DodoServer.GetSMSNumber()}. " +
			"I like Telegram more though, and it means I can be a bit more helpful to you. " +*/
		}

		private string GetWorkingGroupList(User user, EParentGroup pg)
		{
			var site = user.Site;
			var wgs = DodoServer.SiteManager.Data.WorkingGroups.Where(x => x.Value.ParentGroup == pg);
			var sb = new StringBuilder("Select a Working Group to add it to your preferences:\n");
			foreach(var wg in wgs.Where(x => site == null || site.WorkingGroups.Contains(x.Key)))
			{
				sb.AppendLine($"{wg.Value.ShortCode} - {wg.Value.Name} " +
					$"{(user.WorkingGroupPreferences.Contains(wg.Value.ShortCode) ? "✓" : "-")}");
			}
			sb.AppendLine("To change Parent Groups, reply BACK");
			sb.AppendLine("When you're done, reply DONE");
			return sb.ToString();
		}

		string GetParentGroupSelectionString()
		{
			var sb = new StringBuilder("Select a Parent Group to select the Working Groups you can volunteer for.\n");
			foreach(var parentGroup in Enum.GetValues(typeof(EParentGroup)).OfType<EParentGroup>())
			{
				sb.AppendLine($"{(int)parentGroup} - {parentGroup.GetName()}");
			}
			sb.AppendLine("When you're done, reply DONE");
			return sb.ToString();
		}

		string GetSiteList()
		{
			var sb = new StringBuilder("Reply with the site number shown.\n");
			foreach (var site in DodoServer.SiteManager.GetSites())
			{
				if (site.SiteCode== 0)
					{
						continue;
					}
				sb.AppendLine($"{site.SiteCode} - {site.SiteName}");
			}
			sb.AppendLine("Or, if you don't know, reply SKIP.");
			return sb.ToString();
		}

		public override bool CanCancel()
		{
			return false;
		}
	}
}
