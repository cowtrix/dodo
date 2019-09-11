using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	public class CoordinatorNeedsTask : WorkflowTask
	{
		public enum EState
		{
			FirstMessage,
			GettingSiteCode,
			GettingWorkingGroup,
			GettingNumbers,
		}

		public EState State;
		public CoordinatorNeedsManager.Need Need;
		public CoordinatorNeedsTask(Workflow workflow) : base(workflow)
		{
		}

		public override ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			var toUpper = message.Content.ToUpperInvariant().Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
			var user = session.GetUser();
			if (toUpper.Length >= 3 && toUpper.FirstOrDefault() == "NEED")
			{
				var wgCode = toUpper[1];
				Need.WorkingGroup = DodoServer.SiteManager.GetSites().Select(x => x.WorkingGroups).ConcatenateCollection()
					.First(x => x.ShortCode == wgCode);
				Need.SiteCode = Need.WorkingGroup.SiteCode;
				var count = -1;
				if (toUpper[2] == "MANY" || int.TryParse(toUpper[2], out count))
				{
					Need.Amount = count;
					DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, Need);
					ExitTask();
					return new ServerMessage($"You've just requested {(Need.Amount == -1 ? "MANY" : Need.Amount.ToString())} volunteers for site {Need.SiteCode}, {Need.WorkingGroup.Name}. " +
						"You will be hearing from me soon with some details of volunteers to help.");
				}
			}
			
			var cmd = toUpper.FirstOrDefault();
			if (State == EState.FirstMessage)
			{
				State = EState.GettingSiteCode;
				return new ServerMessage(GetSiteNumberRequestString());
			}
			if(cmd == "CANCEL")
			{
				ExitTask();
				return new ServerMessage("Okay, I've cancelled this request.");
			}
			if(State == EState.GettingSiteCode)
			{
				return GetSitecode(message.Content, user);
			}
			if(State == EState.GettingWorkingGroup)
			{
				var content = message.Content;
				var wgs = FilterBySite(user, Need.SiteCode);
				if(!wgs.Any(x => x.ShortCode == cmd))
				{
					return new ServerMessage("Sorry, I didn't understand that Working Group Code." +
						" This is the first two letters shown at the start of each line - of the " +
						"working group that needs volunteers.\n" + GetWorkingGroupRequestString(wgs) +
						" If you'd like to cancel, reply 'CANCEL'.");
				}
				Need.WorkingGroup = wgs.First(x => x.ShortCode == cmd);
				State = EState.GettingNumbers;
				return new ServerMessage("Nice! Now tell me, how many volunteers do you need?" +
					" Reply with a number, or if you just need as many people as possible, reply 'MANY'");
			}
			if (State == EState.GettingNumbers)
			{
				int count = 0;
				if (cmd == "MANY")
				{
					Need.Amount = -1;
				}
				else if (!int.TryParse(cmd, out count))
				{
					return new ServerMessage("Sorry, I didn't understand that number." +
					" Reply with a number, or if you just need as many people as possible, reply 'MANY'."+
					" If you'd like to cancel, reply 'CANCEL'.");
				}
				else
				{
					Need.Amount = count;
				}
			}
			DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, Need);
			ExitTask();
			return new ServerMessage("Thanks, you'll be hearing from me soon with some details of volunteers to help."+ 
				$" In future, you could make this request in one go by saying NEED {Need.WorkingGroup.ShortCode} {(Need.Amount == -1 ? "MANY" : Need.Amount.ToString())}");
		}

		ServerMessage GetSitecode(string content, User user)
		{
			if (!int.TryParse(content, out var siteCode) || !DodoServer.SiteManager.IsValidSiteCode(siteCode))
			{
				return new ServerMessage("Sorry, that didn't seem like a valid Site Number." +
					" If you don't know what your site number is, ask the people around you " +
					"or look for signs.\n" + GetSiteNumberRequestString() + 
					" If you'd like to cancel, reply 'CANCEL'.");
			}
			Need.SiteCode = siteCode;
			State = EState.GettingWorkingGroup;

			var wgs = FilterBySite(user, siteCode);
			if (wgs.Count == 0)
			{
				m_workflow.CurrentTask.ExitTask();
				return new ServerMessage("Sorry, it doesn't look like you're registered as a coordinator " +
					$"at site {siteCode}. Contact the Rebel Support Office to be added." + GetSiteNumberRequestString());
			}
			return new ServerMessage(GetWorkingGroupRequestString(wgs));
		}

		List<WorkingGroup> FilterBySite(User user, int sitecode)
		{
			var wgs = DodoServer.SiteManager.GetSites().Single(x => x.SiteCode == sitecode);
			if (user.SiteCode == 0)
			{
				return wgs.WorkingGroups.ToList();
			}
			return wgs.WorkingGroups.Where(workingGroup => user.CoordinatorRoles
				.Any(role => workingGroup.SiteCode == role.SiteCode)).ToList();
		}

		private string GetSiteNumberRequestString()
		{
			var sb = new StringBuilder("Please tell me the Site Number you would like to request volunteers at:\n");
			foreach (var site in DodoServer.SiteManager.GetSites())
			{
				sb.AppendLine($"{site.SiteCode} - {site.SiteName}");
			}
			return sb.ToString();
		}

		private string GetWorkingGroupRequestString(List<WorkingGroup> wgs)
		{
			var sb = new StringBuilder("Please select the role from the list:\n");
			for (var i = 0; i < wgs.Count; i++)
			{
				var wg = (WorkingGroup)wgs[i];
				sb.AppendLine($"{wg.ShortCode} - {wg.Name}: {wg.ParentGroup}");
			}
			return sb.ToString();
		}
	}
}
