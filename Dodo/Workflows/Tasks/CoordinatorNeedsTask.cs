using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	[WorkflowTaskInfo(EUserAccessLevel.Coordinator)]
	public class CoordinatorNeedsTask : WorkflowTask
	{
		public WorkingGroup WorkingGroup;
		public int Amount = -1;
		public DateTime TimeNeeded;
		public string Description;
		public int SiteCode = -1;
		public EParentGroup? ParentGroupFilter = null;

		public static string CommandKey { get { return "NEED"; } }
		public static string HelpString { get { return $"{CommandKey} - create a Volunteer Request, which means you'll be sent the contact details of volunteers eager to help out."; } }

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			for (int i = 0; i < message.ContentUpper.Length; i++)
			{
				string cmd = message.ContentUpper[i];
				var user = session.GetUser();
				var approvedSites = ApprovedSites(user);
				if(approvedSites.Count == 0)
				{
					ExitTask(session);
					response = new ServerMessage("Sorry, it doesn't look like you're registered as a coordinator at any sites.");
					user.Karma--;
					return true;
				}
				if (approvedSites.Count == 1)
				{
					SiteCode = approvedSites[0].SiteCode;
				}
				if (!DodoServer.SiteManager.IsValidSiteCode(SiteCode))
				{
					if (int.TryParse(cmd, out var siteCode))
					{
						SiteCode = siteCode;
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
						response = new ServerMessage((cmd == CommandKey ? "" : "Sorry, I didn't understand that. If you'd like to cancel, reply 'DONE'. ") +
							GetSiteNumberRequestString(approvedSites));
						return true;
					}
				}

				var workingGroups = ApprovedWorkingGroups(SiteCode, user);
				if (!DodoServer.SiteManager.IsValidWorkingGroup(WorkingGroup) && !DodoServer.SiteManager.IsValidWorkingGroup(cmd))
				{
					if(workingGroups.Count == 1 && user.AccessLevel != EUserAccessLevel.RotaCoordinator)
					{
						WorkingGroup = workingGroups[0];
						var existingNeedCount = DodoServer.CoordinatorNeedsManager.GetNeedsForWorkingGroup(SiteCode, WorkingGroup).Count();
						if (existingNeedCount >= CoordinatorNeedsManager.MaxNeedCountPerWorkingGroup)
						{
							response = new ServerMessage($"Sorry, the working group {WorkingGroup.Name} already has the maximum amount of Volunteer Requests. " +
								$"To be able to add a new one, you must remove an existing one. Do this by saying {CoordinatorRemoveNeedTask.CommandKey}");
							ExitTask(session);
							return true;
						}
					}
					else
					{
						// Filter by parent group
						if (ParentGroupFilter == null)
						{
							var list = Enum.GetValues(typeof(EParentGroup)).OfType<EParentGroup>().ToList();
							if (int.TryParse(cmd, out var number) && number >= 0 && number < list.Count 
								&& (SiteCode > 0 || list[number] < EParentGroup.RSO))
							{
								ParentGroupFilter = list.ElementAt(number);
								if (i >= message.ContentUpper.Length - 1)
								{
									var filteredGroups = workingGroups.Where(x => x.ParentGroup == ParentGroupFilter.Value);
									if (filteredGroups.Count() == 0)
									{
										response = new ServerMessage($"Okay, you selected {ParentGroupFilter.Value.GetName()}. " +
											"It doesn't look like there are any coordinators registered for this Parent Group at this site.");
										ExitTask(session);
										return true;
									}
									response = new ServerMessage($"Okay, you selected {ParentGroupFilter.Value.GetName()}. "
												+ GetWorkingGroupRequestString(filteredGroups));
									return true;
								}
								continue;
							}
							else if (i >= message.ContentUpper.Length - 1)
							{
								var sb = new StringBuilder("Please tell me which Parent Group the working group belongs to:\n");
								for (int j = 0; j < list.Count; j++)
								{
									if (SiteCode > 0 && list[j] == EParentGroup.RSO)
									{
										continue;
									}
									sb.AppendLine($"{j} - {list[j].GetName()}");
								}
								sb.AppendLine("Or, if you already know the working group code, you can just tell me that.");
								response = new ServerMessage(sb.ToString());
								return true;
							}
							else if (i >= message.ContentUpper.Length - 1)
							{
								var filteredGroups = workingGroups.Where(x => x.ParentGroup == ParentGroupFilter.Value);
								if(filteredGroups.Count() == 0)
								{
									response = new ServerMessage($"Okay, you selected {ParentGroupFilter.Value.GetName()}. " +
										"It doesn't look like there are any coordinators registered for this Parent Group at this site.");
									ExitTask(session);
									return true;
								}
								response = new ServerMessage($"Okay, you selected {ParentGroupFilter.Value.GetName()}. "
											+ GetWorkingGroupRequestString(filteredGroups));
								return true;
							}
							continue;
						}
						else
						{
							workingGroups = workingGroups.Where(x => x.ParentGroup == ParentGroupFilter.Value).ToList();
							if (workingGroups.Count() == 0)
							{
								response = new ServerMessage($"Okay, you selected {ParentGroupFilter.Value.GetName()}. " +
									"It doesn't look like there are any coordinators registered for this Parent Group at this site.");
								ExitTask(session);
								return true;
							}
						}
					}
				}

				if (!DodoServer.SiteManager.IsValidWorkingGroup(WorkingGroup))
				{
					if (workingGroups.Any(x => x.ShortCode == cmd))
					{
						WorkingGroup = workingGroups.First(x => x.ShortCode == cmd);
						var existingNeedCount = DodoServer.CoordinatorNeedsManager.GetNeedsForWorkingGroup(SiteCode, WorkingGroup).Count();
						if (existingNeedCount >= CoordinatorNeedsManager.MaxNeedCountPerWorkingGroup)
						{
							response = new ServerMessage($"Sorry, the working group {WorkingGroup.Name} already has the maximum amount of Volunteer Requests. " +
								$"To be able to add a new one, you must remove an existing one. Do this by saying {CoordinatorRemoveNeedTask.CommandKey}");
							ExitTask(session);
							return true;
						}
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
							WorkingGroup = workingGroups[0];
							var existingNeedCount = DodoServer.CoordinatorNeedsManager.GetNeedsForWorkingGroup(SiteCode, WorkingGroup).Count();
							if (existingNeedCount >= CoordinatorNeedsManager.MaxNeedCountPerWorkingGroup)
							{
								response = new ServerMessage($"Sorry, the working group {WorkingGroup.Name} already has the maximum amount of Volunteer Requests. " +
									$"To be able to add a new one, you must remove an existing one. Do this by saying {CoordinatorRemoveNeedTask.CommandKey}");
								ExitTask(session);
								return true;
							}
						}
						else
						{
							response = new ServerMessage("Sorry, I didn't understand that. If you'd like to cancel, reply 'DONE'. " +
								GetWorkingGroupRequestString(workingGroups));
							return true;
						}
					}
				}

				if (TimeNeeded == default)
				{
					var timeCommand = message.ContentUpper.Skip(i).Aggregate("", (f, s) => f + " " + s).Trim();
					if (timeCommand.StartsWith("NOW"))
					{
						TimeNeeded = DateTime.MaxValue;
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
						var maxStr = "";
						if(date - DateTime.Now < TimeSpan.FromHours(6))
						{
							TimeNeeded = DateTime.MaxValue;
							maxStr = "It looks like that's within the next few hours - it's better to use NOW when that's the case. I've changed that for you. ";
						}
						else
						{
							TimeNeeded = date;
						}						
						if (i >= message.ContentUpper.Length - 2)
						{
							response = new ServerMessage(maxStr + "Now, tell me how many volunteers you need." +
								" Reply with a number, or if you just need as many people as possible, reply 'MANY'." +
								" If you'd like to cancel, reply 'DONE'.");
							return true;
						}
						++i;
						continue;
					}
					else
					{
						if(i >= message.ContentUpper.Length - 1)
						{
							response = new ServerMessage((cmd == CommandKey ? "" : "Sorry, I didn't understand that. If you'd like to cancel, reply 'DONE'. ")
								+ GetTimeRequestString());
							return true;
						}
						continue;
					}
				}

				if (Amount < 0)
				{
					int count = 0;
					if (cmd == "MANY")
					{
						Amount = int.MaxValue;
					}
					else if (cmd == CommandKey)
					{
						response = new ServerMessage(GetNumberRequest());
						return true;
					}
					else if (!int.TryParse(cmd, out count) || count < 0 || count > 99)
					{
						if (i >= message.ContentUpper.Length - 1)
						{
							response = new ServerMessage("Sorry, I didn't understand that number." +
								" Reply with a number, or if you just need as many people as possible, reply 'MANY'." +
								" If you'd like to cancel, reply 'DONE'.");
							return true;
						}
						continue;
					}
					else
					{
						Amount = count;
					}
				}

				if(Description == null)
				{
					response = new ServerMessage("Now, if you'd like, you can tell me in 160 characters or less what the role is. Or, to skip this step, reply SKIP");
					Description = "";
					return true;
				}
				if(Description == "" && message.ContentUpper.FirstOrDefault() != "SKIP")
				{
					if(message.Content.Length > 160)
					{
						response = new ServerMessage("Sorry, that was too long. If you'd like to cancel, reply 'DONE'. Or, to skip this step, reply SKIP");
						return true;
					}
					Description = message.Content;
				}
				ExitTask(session);
				string key = null;
				if (!DodoServer.CoordinatorNeedsManager.AddNeedRequest(user, WorkingGroup, SiteCode, Amount, TimeNeeded, Description, out key))
				{
					response = new ServerMessage("Sorry, there was a problem adding that Volunteer Request. Please try again.");
					return true;
				}
				// "NEED 0 AD 7/10 08:00 3"
				response = new ServerMessage("Thanks, you'll be hearing from me soon with some details of volunteers to help." +
					$" The code for this Volunteer Request is {key}." +
					$" In future, you could make this request in one go by saying {CoordinatorNeedsTask.CommandKey} " +
					(user.AccessLevel >= EUserAccessLevel.RSO ? SiteCode + " " : "") +
					(user.AccessLevel >= EUserAccessLevel.RotaCoordinator ? WorkingGroup.ShortCode + " " : "") +
					$"{ Utility.ToDateTimeCode(TimeNeeded)} {(Amount == int.MaxValue ? "MANY" : Amount.ToString())}");
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

		private List<WorkingGroup> ApprovedWorkingGroups(int siteCode, User user)
		{
			if (user.AccessLevel == EUserAccessLevel.Coordinator)
			{
				return user.CoordinatorRoles.Select(x => x.WorkingGroup).ToList();
			}
			var site = DodoServer.SiteManager.GetSite(siteCode);
			var allWgs = DodoServer.SiteManager.Data.WorkingGroups.Values
				.Where(x => site.WorkingGroups.Contains(x.ShortCode));
			if (user.AccessLevel == EUserAccessLevel.RotaCoordinator)
			{
				return allWgs.Where(x => x.ParentGroup != EParentGroup.RSO).ToList();
			}
			return allWgs.ToList();
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
			bool needHeaders = wgs.Any(x => x.ParentGroup != parentGroup);
			for (var i = 0; i < wgs.Count(); i++)
			{
				var wg = (WorkingGroup)wgs.ElementAt(i);
				if(needHeaders && (i == 0 || wg.ParentGroup != parentGroup))
				{
					parentGroup = wg.ParentGroup;
					sb.AppendLine($"{parentGroup}:");
				}
				sb.AppendLine($"		{wg.ShortCode} - {wg.Name}");
			}
			return sb.ToString();
		}
	}
}
