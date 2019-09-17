using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	public class CoordinatorNeedsTask : WorkflowTask
	{
		public CoordinatorNeedsManager.Need Need = new CoordinatorNeedsManager.Need();

		public static string CommandKey { get { return "NEED"; } }

		public CoordinatorNeedsTask(Workflow workflow) : base(workflow)
		{
		}

		public override ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			var toUpper = message.Content.ToUpperInvariant()
				.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < toUpper.Length; i++)
			{
				string cmd = (string)toUpper[i];
				if (cmd == "CANCEL")
				{
					ExitTask();
					return new ServerMessage("Okay, I've canceled this request.");
				}
				var user = session.GetUser();
				var approvedSites = ApprovedSites(user);
				if (approvedSites.Count == 1)
				{
					Need.SiteCode = approvedSites[0].SiteCode;
				}
				if (!DodoServer.SiteManager.IsValidSiteCode(Need.SiteCode))
				{
					if (int.TryParse(cmd, out var siteCode))
					{
						Need.SiteCode = siteCode;
					}
					else
					{
						return new ServerMessage(GetSiteNumberRequestString(approvedSites));
					}
				}

				var workingGroups = ApprovedWorkingGroups(user);
				if (!DodoServer.SiteManager.IsValidWorkingGroup(Need.WorkingGroup))
				{
					if (workingGroups.Any(x => x.ShortCode == cmd))
					{
						Need.WorkingGroup = workingGroups.First(x => x.ShortCode == cmd);
						return new ServerMessage("Nice! Now tell me, how many volunteers do you need?" +
							" Reply with a number, or if you just need as many people as possible, reply 'MANY'");
					}
					else
					{
						if (workingGroups.Count == 1)
						{
							Need.WorkingGroup = workingGroups[0];
						}
						else
						{
							return new ServerMessage(GetWorkingGroupRequestString(workingGroups));
						}
					}
				}

				if (Need.TimeNeeded == default)
				{
					var timeCommand = toUpper.Skip(i).Aggregate("", (f, s) => f + " " + s);
					if (timeCommand.StartsWith("NOW"))
					{
						Need.TimeNeeded = DateTime.Now;
						if (i >= toUpper.Length - 1)
						{
							return GetNumberRequest();
						}
						continue;
					}
					else if (Utility.TryParseDateTime(timeCommand, out var date))
					{
						if(date < DateTime.Now)
						{
							return new ServerMessage($"Sorry, you can only request volunteers for the future.");
						}
						Need.TimeNeeded = date;
					}
					else
					{
						if(i >= toUpper.Length - 1)
						{
							return new ServerMessage($"Tell me when you need these volunteers. You can specify a date and a 24-hour time. For instance, reply '7/10 16:00'" +
							" to ask for volunteers on the 7th of October at 4pm. If you need volunteers now, reply NOW");
						}
						continue;
					}
				}

				int count = 0;
				if (cmd == "MANY")
				{
					Need.Amount = int.MaxValue;
				}
				else if (cmd == "NEED")
				{
					return GetNumberRequest();
				}
				else if (!int.TryParse(cmd, out count))
				{
					if (i >= toUpper.Length - 1)
					{
						return new ServerMessage("Sorry, I didn't understand that number." +
						" Reply with a number, or if you just need as many people as possible, reply 'MANY'." +
						" If you'd like to cancel, reply 'CANCEL'.");
					}
					continue;
				}
				else
				{
					Need.Amount = count;
				}

				DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, Need);
				ExitTask();
				return new ServerMessage("Thanks, you'll be hearing from me soon with some details of volunteers to help." +
					$" In future, you could make this request in one go by saying NEED " +
					$"{Need.WorkingGroup.ShortCode}{Need.SiteCode} {(Need.Amount == int.MaxValue ? "MANY" : Need.Amount.ToString())} {Utility.ToDateTimeCode(Need.TimeNeeded)}");
			}
			return default;
		}

		private ServerMessage GetNumberRequest()
		{
			return new ServerMessage($"You're requesting volunteers for {Need.WorkingGroup.Name} at site {Need.SiteCode}. " +
						"Tell me how many volunteers you need, or if you just need as many people as possible, reply 'MANY'.");
		}

		private List<SiteSpreadsheet> ApprovedSites(User user)
		{
			var allSites = DodoServer.SiteManager.GetSites();
			if (user.AccessLevel == EUserAccessLevel.RSO)
			{
				return allSites;
			}
			return allSites.Where(x => user.CoordinatorRoles.Any(y => y.SiteCode == x.SiteCode)).ToList();
		}

		private List<WorkingGroup> ApprovedWorkingGroups(User user)
		{
			if (user.AccessLevel == EUserAccessLevel.Coordinator)
			{
				return user.CoordinatorRoles.Select(x => x.WorkingGroup).ToList();
			}
			return DodoServer.SiteManager.WorkingGroups.Values.ToList();
		}

		private string GetSiteNumberRequestString(List<SiteSpreadsheet> sites)
		{
			var sb = new StringBuilder("Please tell me the Site Number you would like to request volunteers at:\n");
			foreach (var site in sites)
			{
				sb.AppendLine($"{site.SiteCode} - {site.SiteName}");
			}
			return sb.ToString();
		}

		private string GetWorkingGroupRequestString(List<WorkingGroup> wgs)
		{
			var sb = new StringBuilder("Please select the Working Group from the list:\n");
			var parentGroup = wgs.First().ParentGroup;
			for (var i = 0; i < wgs.Count; i++)
			{
				var wg = (WorkingGroup)wgs[i];
				if(i == 0 || wg.ParentGroup != parentGroup)
				{
					parentGroup = wg.ParentGroup;
					sb.AppendLine($"{parentGroup}:");
				}
				sb.AppendLine($"        {wg.ShortCode} - {wg.Name}");
			}
			return sb.ToString();
		}
	}
}
