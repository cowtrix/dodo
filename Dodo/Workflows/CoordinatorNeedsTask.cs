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

		List<SiteSpreadsheet> ApprovedSites(User user)
		{
			var allSites = DodoServer.SiteManager.GetSites();
			if(user.AccessLevel == EUserAccessLevel.RSO)
			{
				return allSites;
			}
			return allSites.Where(x => user.CoordinatorRoles.Any(y => y.SiteCode == x.SiteCode)).ToList();
		}

		List<WorkingGroup> ApprovedWorkingGroups(User user)
		{
			if (user.AccessLevel == EUserAccessLevel.Coordinator)
			{
				return user.CoordinatorRoles.ToList();
			}
			if(user.AccessLevel == EUserAccessLevel.RotaCoordinator)
			{
				return DodoServer.SiteManager.GetSites().Single(x => x.SiteCode == user.SiteCode)
					.WorkingGroups.ToList();
			}
			return DodoServer.SiteManager.GetSites().Select(x => x.WorkingGroups)
				.ConcatenateCollection().ToList();
		}

		public override ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			var toUpper = message.Content.ToUpperInvariant()
				.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
			foreach(var cmd in toUpper)
			{
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

				var roles = ApprovedWorkingGroups(user);
				if (!DodoServer.SiteManager.IsValidWorkingGroup(Need.WorkingGroup))
				{
					if (roles.Any(x => x.ShortCode == cmd))
					{
						Need.WorkingGroup = roles.First(x => x.ShortCode == cmd);
						return new ServerMessage("Nice! Now tell me, how many volunteers do you need?" +
							" Reply with a number, or if you just need as many people as possible, reply 'MANY'");
					}
					else
					{
						if (roles.Count == 1)
						{
							Need.WorkingGroup = roles[0];
						}
						else
						{
							return new ServerMessage(GetWorkingGroupRequestString(
								roles.Where(x => x.SiteCode == Need.SiteCode).ToList()));
						}
					}
				}

				if (Need.TimeNeeded == default(DateTime))
				{
					if (cmd == "NOW")
					{
						Need.TimeNeeded = DateTime.Now;
					}
					else if (Utility.TryParseDateTime(message.Content, out var date))
					{
						if(date < DateTime.Now)
						{
							return new ServerMessage($"Sorry, you can only request volunteers for the future.");
						}
						Need.TimeNeeded = date;
					}
					else
					{
						return new ServerMessage($"Tell me when you need these volunteers. You can specify a date and a 24-hour time. For instance, reply '7/10 16:00' to ask for volunteers on the 7th of October at 4pm. If you need volunteers now, reply NOW");
					}
				}

				int count = 0;
				if (cmd == "MANY")
				{
					Need.Amount = int.MaxValue;
				}
				else if (cmd == "NEED")
				{
					return new ServerMessage($"You're requesting volunteers for {Need.WorkingGroup.Name} at site {Need.SiteCode}. " +
						"Tell me how many volunteers you need, or if you just need as many people as possible, reply 'MANY'.");
				}
				else if (!int.TryParse(cmd, out count))
				{
					return new ServerMessage("Sorry, I didn't understand that number." +
						" Reply with a number, or if you just need as many people as possible, reply 'MANY'." +
						" If you'd like to cancel, reply 'CANCEL'.");
				}
				else
				{
					Need.Amount = count;
				}
				DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, Need);
				ExitTask();
				return new ServerMessage("Thanks, you'll be hearing from me soon with some details of volunteers to help." +
					$" In future, you could make this request in one go by saying NEED " +
					$"{Need.WorkingGroup.ShortCode}{Need.WorkingGroup.SiteCode} {(Need.Amount == int.MaxValue ? "MANY" : Need.Amount.ToString())} {Utility.ToDateTimeCode(Need.TimeNeeded)}");
			}
			return default;
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
