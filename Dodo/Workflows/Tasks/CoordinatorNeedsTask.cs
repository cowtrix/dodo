using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	public class CoordinatorNeedsTask : WorkflowTask
	{
		public CoordinatorNeedsManager.Need Need = new CoordinatorNeedsManager.Need();
		public EParentGroup? ParentGroupFilter = null;

		public static string CommandKey { get { return "NEED"; } }

		public CoordinatorNeedsTask(Workflow workflow) : base(workflow)
		{
		}

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			for (int i = 0; i < message.ContentUpper.Length; i++)
			{
				string cmd = message.ContentUpper[i];
				var user = session.GetUser();
				var approvedSites = ApprovedSites(user);
				if(approvedSites.Count == 0)
				{
					ExitTask();
					response = new ServerMessage("Sorry, it doesn't look like you're registered as a coordinator at any sites.");
					user.Karma--;
					return true;
				}
				if (approvedSites.Count == 1)
				{
					Need.SiteCode = approvedSites[0].SiteCode;
				}
				if (!DodoServer.SiteManager.IsValidSiteCode(Need.SiteCode))
				{
					if (int.TryParse(cmd, out var siteCode))
					{
						Need.SiteCode = siteCode;
						if (i >= message.ContentUpper.Length - 1)
						{
							cmd = "XXXX";
						}
						else
						{
							continue;
						}
					}
					else if (i < message.ContentUpper.Length - 1)
					{
						continue;
					}
					else
					{
						response = new ServerMessage(GetSiteNumberRequestString(approvedSites));
						return true;
					}
				}

				var workingGroups = ApprovedWorkingGroups(user);
				if (!DodoServer.SiteManager.IsValidWorkingGroup(Need.WorkingGroup) && !DodoServer.SiteManager.IsValidWorkingGroup(cmd))
				{
					if(workingGroups.Count == 1 && user.AccessLevel != EUserAccessLevel.RotaCoordinator)
					{
						Need.WorkingGroup = workingGroups[0];
					}
					else
					{
						// Filter by parent group
						if (ParentGroupFilter == null)
						{
							var list = Enum.GetValues(typeof(EParentGroup)).OfType<EParentGroup>().ToList();
							if (!SiteSpreadsheetManager.TryStringToParentGroup(cmd, out var parentGroup))
							{
								if (int.TryParse(cmd, out var number) && number >= 0 && number < list.Count)
								{
									ParentGroupFilter = list.ElementAt(number);
									if (i >= message.ContentUpper.Length - 1)
									{
										response = new ServerMessage($"Okay, you selected {ParentGroupFilter.Value}. "
											+ GetWorkingGroupRequestString(workingGroups.Where(x => x.ParentGroup == ParentGroupFilter.Value)));
										return true;
									}
									continue;
								}
								if (i >= message.ContentUpper.Length - 1)
								{
									var sb = new StringBuilder("Please tell me which Parent Group the working group belongs to:\n");
									for (int j = 0; j < list.Count; j++)
									{
										if (list[j] == EParentGroup.RSO)
										{
											continue;
										}
										sb.AppendLine($"{j} - {list[j]}");
									}
									sb.AppendLine("Or, if you already know the working group code, you can just tell me that.");
									response = new ServerMessage(sb.ToString());
									return true;
								}
								continue;
							}
							ParentGroupFilter = parentGroup;
							if (i >= message.ContentUpper.Length - 1)
							{
								response = new ServerMessage($"Okay, you selected {ParentGroupFilter.Value}. "
											+ GetWorkingGroupRequestString(workingGroups.Where(x => x.ParentGroup == ParentGroupFilter.Value)));
								return true;
							}
							continue;
						}
						else
						{
							workingGroups = workingGroups.Where(x => x.ParentGroup == ParentGroupFilter.Value).ToList();
						}
					}
				}

				if (!DodoServer.SiteManager.IsValidWorkingGroup(Need.WorkingGroup))
				{
					if (workingGroups.Any(x => x.ShortCode == cmd))
					{
						Need.WorkingGroup = workingGroups.First(x => x.ShortCode == cmd);
						if (i >= message.ContentUpper.Length - 1)
						{
							response = new ServerMessage(GetTimeRequestString());
							return true;
						}
						continue;
					}
					else
					{
						if (workingGroups.Count == 1)
						{
							Need.WorkingGroup = workingGroups[0];
						}
						else
						{
							response = new ServerMessage(GetWorkingGroupRequestString(workingGroups));
							return true;
						}
					}
				}

				if (Need.TimeNeeded == default)
				{
					var timeCommand = message.ContentUpper.Skip(i).Aggregate("", (f, s) => f + " " + s);
					if (timeCommand.StartsWith("NOW"))
					{
						Need.TimeNeeded = DateTime.Now;
						if (i >= message.ContentUpper.Length - 1)
						{
							response = new ServerMessage(GetNumberRequest());
							return true;
						}
						continue;
					}
					else if (Utility.TryParseDateTime(timeCommand, out var date))
					{
						if(date < DateTime.Now)
						{
							response = new ServerMessage($"Sorry, you can only request volunteers for the future.");
							return true;
						}
						Need.TimeNeeded = date;
						if (i >= message.ContentUpper.Length - 2)
						{
							response = new ServerMessage("Now, tell me how many volunteers you need." +
								" Reply with a number, or if you just need as many people as possible, reply 'MANY'." +
								" If you'd like to cancel, reply 'CANCEL'.");
							return true;
						}
						++i;
						continue;
					}
					else
					{
						if(i >= message.ContentUpper.Length - 1)
						{
							response = new ServerMessage(GetTimeRequestString());
							return true;
						}
						continue;
					}
				}

				int count = 0;
				if (cmd == "MANY")
				{
					Need.Amount = int.MaxValue;
				}
				else if (cmd == CommandKey)
				{
					response = new ServerMessage(GetNumberRequest());
					return true;
				}
				else if (!int.TryParse(cmd, out count))
				{
					if (i >= message.ContentUpper.Length - 1)
					{
						response = new ServerMessage("Sorry, I didn't understand that number." +
							" Reply with a number, or if you just need as many people as possible, reply 'MANY'." +
							" If you'd like to cancel, reply 'CANCEL'.");
						return true;
					}
					continue;
				}
				else
				{
					Need.Amount = count;
				}

				DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, Need);
				ExitTask();
				// "NEED 0 AD 7/10 08:00 3"
				response = new ServerMessage("Thanks, you'll be hearing from me soon with some details of volunteers to help." +
					$" In future, you could make this request in one go by saying NEED " +
					$"{Need.SiteCode} {Need.WorkingGroup.ShortCode} {Utility.ToDateTimeCode(Need.TimeNeeded)} {(Need.Amount == int.MaxValue ? "MANY" : Need.Amount.ToString())}");
				return true;
			}
			response = default;
			return false;
		}

		private string GetTimeRequestString()
		{
			return "Tell me when you need these volunteers. You can specify a date and a 24-hour time. For instance, reply '7/10 16:00'" +
								" to ask for volunteers on the 7th of October at 4pm. If you need volunteers now, reply NOW";
		}

		private string GetNumberRequest()
		{
			return "Tell me how many volunteers you need, or if you just need as many people as possible, reply 'MANY'.";
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
			return DodoServer.SiteManager.Data.WorkingGroups.Values
				.Where(x => x.ParentGroup != EParentGroup.RSO).ToList();
		}

		private string GetSiteNumberRequestString(IEnumerable<SiteSpreadsheet> sites)
		{
			var sb = new StringBuilder("Please tell me the Site Number you would like to request volunteers at:\n");
			foreach (var site in sites)
			{
				sb.AppendLine($"{site.SiteCode} - {site.SiteName}");
			}
			return sb.ToString();
		}

		private string GetWorkingGroupRequestString(IEnumerable<WorkingGroup> wgs)
		{
			var sb = new StringBuilder("Please select the Working Group from the list:\n");
			var parentGroup = wgs.First().ParentGroup;
			for (var i = 0; i < wgs.Count(); i++)
			{
				var wg = (WorkingGroup)wgs.ElementAt(i);
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
