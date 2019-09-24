using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	[WorkflowTaskInfo(EUserAccessLevel.Coordinator)]
	public class CoordinatorRemoveNeedTask : WorkflowTask
	{
		public CoordinatorNeedsManager.Need Need = null;

		public static string CommandKey { get { return "DELETENEED"; } }
		public static string HelpString { get { return $"{CommandKey} - delete a Volunteer Request that you or someone from your working group has made."; } }

		List<CoordinatorNeedsManager.Need> GetNeeds(User user)
		{
			var needs = DodoServer.CoordinatorNeedsManager.GetCurrentNeeds();
			if (user.AccessLevel == EUserAccessLevel.RSO)
			{
				return needs;
			}
			if(user.AccessLevel == EUserAccessLevel.RotaCoordinator)
			{
				return needs.Where(x => user.CoordinatorRoles.Any(y => y.SiteCode == x.SiteCode)).OrderBy(x => x.TimeOfRequest).ToList();
			}
			if (user.AccessLevel == EUserAccessLevel.Coordinator)
			{
				return needs.Where(x => user.CoordinatorRoles.Any(y => y.WorkingGroup.ShortCode == x.WorkingGroupCode)).OrderBy(x => x.TimeOfRequest).ToList();
			}
			throw new Exception("Bad user: " + user.UUID);
		}

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var toUpper = message.Content.ToUpperInvariant()
				.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < toUpper.Length; i++)
			{
				string cmd = (string)toUpper[i];
				var user = session.GetUser();
				if(Need == null)
				{
					var authorisedNeeds = GetNeeds(user);
					if (DodoServer.CoordinatorNeedsManager.CurrentNeeds.TryGetValue(cmd, out var pick))
					{
						if(authorisedNeeds.Contains(pick))
						{
							Need = pick;
							response = Finalize(session);
						}
						else
						{
							response = new ServerMessage("Sorry, it doesn't look like you're allowed to do that.");
							ExitTask(session);
						}
						return true;
					}
					if (authorisedNeeds.Count == 0)
					{
						response = new ServerMessage($"It doesn't look like your Working Group{(user.CoordinatorRoles.Count > 1 ? "s" : "")}" +
							$" have any open Volunteer Requests. You can make a Volunteer Request with the {CoordinatorNeedsTask.CommandKey} command.");
						ExitTask(session);
						return true;
					}
					if(cmd == "PREV")
					{
						m_page--;
					}
					if(cmd == "NEXT")
					{
						m_page++;
					}
					string didntUnderstand = "";
					if(cmd != CommandKey)
					{
						didntUnderstand = "Sorry, I didn't understand that code. If you'd like to cancel, reply DONE. ";
					}
					response = new ServerMessage(didntUnderstand + GetNeedsMenu(authorisedNeeds));
					return true;
				}

			}
			response = default;
			return false;
		}

		private ServerMessage Finalize(UserSession session)
		{
			ExitTask(session);
			if (DodoServer.CoordinatorNeedsManager.RemoveNeed(Need))
				return new ServerMessage("Great, I've removed that Volunteer Request from the system.");
			else
				return new ServerMessage("Sorry, that didn't work for some reason. Please try again.");
		}

		int m_page = 1;
		const int MaxNeedsPerPage = 10;
		private string GetNeedsMenu(List<CoordinatorNeedsManager.Need> needs)
		{
			var totalPages = needs.Count / MaxNeedsPerPage + 1;
			var sb = new StringBuilder("Please tell me the code for the Volunteer Request to cancel:\n");
			var needIter = needs.OrderBy(x => x.SiteCode).ToList();
			var siteCode = needIter.First().SiteCode;
			var needSiteCode = needs.Any(x => x.SiteCode != siteCode);
			var startIndex = ((m_page - 1) % totalPages) * MaxNeedsPerPage;
			for (int i = startIndex; i < Math.Min(needIter.Count, startIndex + MaxNeedsPerPage); i++)
			{
				CoordinatorNeedsManager.Need need = needIter[i];
				if (needSiteCode && siteCode != need.SiteCode || i == startIndex)
				{
					siteCode = need.SiteCode;
					sb.AppendLine($"==Site {siteCode}=====");
				}
				sb.AppendLine($"{need.Key} > {need.WorkingGroup.Name} - {(need.Amount == int.MaxValue ? "MANY" : need.Amount.ToString())} for {Utility.ToDateTimeCode(need.TimeNeeded)}");
			}
			if(totalPages > 1)
			{
				if(m_page > 1)
				{
					sb.Append("◀reply PREV - ");
				}
				sb.Append($"pg{m_page}/{totalPages}");
				if(m_page < totalPages)
				{
					sb.Append(" - reply NEXT▶");
				}
			}
			return sb.ToString();
		}
	}
}
