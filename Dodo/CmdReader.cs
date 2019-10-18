using System.Threading.Tasks;
using System.IO;
using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Common;

namespace XR.Dodo
{
	public class CmdReader
	{
		public class Command
		{
			public Func<string, Task> Action;
		}
		private ConcurrentDictionary<string, Command> m_commands = new ConcurrentDictionary<string, Command>();

		private string OutputPath
		{
			get
			{
				return Path.GetFullPath(DodoCmd.Cmd.OutputPath);
			}
		}

		private string InputPath
		{
			get
			{
				return Path.GetFullPath(DodoCmd.Cmd.InputPath);
			}
		}

		public CmdReader()
		{
			AddCommand("t", (async x => {
				var response = DodoServer.TelegramGateway.FakeMessage(x, 999999);
				Output(response.Content);
				}));
			AddCommand("s", (async x =>
			{
				var response = DodoServer.SMSGateway.FakeMessage(x, "447856465191");
				Output(response.Content);
			}));
			AddCommand("erroremails", (async x =>
			{
				DodoServer.SiteManager.GenerateErrorEmails(x);
			}));
			AddCommand("backup", (async x =>
			{
				DodoServer.Backup();
			}));
			AddCommand("load", (async x =>
			{
				DodoServer.Load(x);
			}));
			AddCommand("saveconfig", (async x =>
			{
				DodoServer.SaveConfig();
			}));
			AddCommand("shutdown", (async x =>
			{
				DodoServer.Backup();
				Environment.Exit(0);
			}));
			AddCommand("deleteuser", (async x =>
			{
				var user = DodoServer.SessionManager.GetUserFromUserID(x);
				if(user == null)
				{
					Output("Couldn't find user");
				}
				DodoServer.SessionManager.RemoveUser(user);
			}));
			AddCommand("searchuser", (async x =>
			{
				var users = DodoServer.SessionManager.GetUsers()
					.Where(u => (u.Name ?? "").ToUpperInvariant().Contains(x.ToUpperInvariant()) || u.TelegramUser.ToString().Contains(x) || 
					(u.UUID ?? "").Contains(x) || (u.Email ?? "").ToUpperInvariant().Contains(x.ToUpperInvariant()) || (u.PhoneNumber ?? "").Contains(x) ||
					x.ToUpperInvariant() == u.AccessLevel.ToString().ToUpperInvariant());
				if(!users.Any())
				{
					Output("No matches found");
					return;
				}
				var sb = new StringBuilder();
				foreach(var user in users)
				{
					sb.AppendLine($"{user.UUID}: {user}");
				}
				Output(sb.ToString());
			}));
			AddCommand("conv", (async x =>
			{
				var user = DodoServer.SessionManager.GetUserFromUserID(x);
				if (user == null)
				{
					Output("Couldn't find user");
				}
				var session = DodoServer.SessionManager.GetOrCreateSession(user);
				var ins = session.Inbox.Select(inMsg => new { TimeStamp = inMsg.TimeStamp, Content = inMsg.Content, Type = "<<" });
				var outs = session.Outbox.Select(outMsg => new { TimeStamp = outMsg.TimeStamp, Content = outMsg.Content, Type = ">>" });
				var all = ins.Concat(outs).OrderBy(msg => msg.TimeStamp);
				var sb = new StringBuilder(user.ToString() + "\n");
				foreach (var msg in all)
				{
					sb.AppendLine($"{msg.TimeStamp} {msg.Type} {msg.Content}");
				}
				Output(sb.ToString());
			}));
			AddCommand("user", (async x =>
			{
				var user = DodoServer.SessionManager.GetUserFromUserID(x);
				if (user == null)
				{
					Output("Couldn't find user");
				}
				var session = DodoServer.SessionManager.GetOrCreateSession(user);
				var sb = new StringBuilder(user.UUID + "\n");
				sb.AppendLine($"Name: {user.Name}");
				sb.AppendLine($"Email: {user.Email}");
				sb.AppendLine($"Ph: {user.PhoneNumber}");
				sb.AppendLine($"Site: {user.SiteCode} {user.Site?.SiteName}");
				sb.AppendLine($"Telegram: {user.TelegramUser}");
				sb.AppendLine($"Access Level: {user.AccessLevel}");
				foreach(var role in user.CoordinatorRoles)
				{
					sb.AppendLine($"Role: {role.Name}");
					sb.AppendLine($"\tSite: {role.SiteCode}");
					sb.AppendLine($"\tWorking Group: {role.WorkingGroupCode}");
				}
				Output(sb.ToString());
			}));
			AddCommand("deletecheckin", (async x =>
			{
				var user = DodoServer.SessionManager.GetUserFromUserID(x);
				if (user == null)
				{
					Output("Couldn't find user");
				}
				DodoServer.CoordinatorNeedsManager.Data.Checkins.TryRemove(user.UUID, out _);
			}));
			AddCommand("coordemails", (async x =>
			{
				var users = DodoServer.SessionManager.GetUsers()
					.Where(user => user.AccessLevel >= EUserAccessLevel.Coordinator && !user.IsVerified() &&
					!string.IsNullOrEmpty(user.Email));
				var sb = new StringBuilder();
				foreach (var user in users)
				{
					sb.AppendLine($"{user.Email} - {user.Name} ({user.PhoneNumber})");
				}
				Output(sb.ToString());
			}));
			AddCommand("stats", (async x =>
			{
				var users = DodoServer.SessionManager.GetUsers();
				var sb = new StringBuilder();
				sb.AppendLine($"Total Users: {users.Count}");
				sb.AppendLine($"Coordinators: {users.Where(user => user.AccessLevel > EUserAccessLevel.Volunteer).Count()} (Active: {users.Where(user => user.AccessLevel > EUserAccessLevel.Volunteer && user.Active).Count()})");
				sb.AppendLine($"Volunteers: {users.Where(user => user.AccessLevel == EUserAccessLevel.Volunteer).Count()} (Active: {users.Where(user => user.AccessLevel == EUserAccessLevel.Volunteer && user.Active).Count()})");
				sb.AppendLine($"Total Msg In: {users.Sum(user => DodoServer.SessionManager.GetOrCreateSession(user).Inbox.Count)}");
				sb.AppendLine($"Total Msg Out: {users.Sum(user => DodoServer.SessionManager.GetOrCreateSession(user).Outbox.Count)}");
				sb.AppendLine($"Volunteer Requests Broadcast: {DodoServer.CoordinatorNeedsManager.Data.TotalRequestsSent}");
				sb.AppendLine($"Volunteer Requests Confirmed: {DodoServer.CoordinatorNeedsManager.Data.TotalRequestsAccepted}");
				sb.AppendLine($"Hit Rate: {DodoServer.CoordinatorNeedsManager.Data.TotalRequestsAccepted / (float)DodoServer.CoordinatorNeedsManager.Data.TotalRequestsSent}");
				sb.AppendLine($"Requests Completed: {DodoServer.CoordinatorNeedsManager.Data.VolunteerRequestsCompleted}");
				sb.AppendLine($"0 contacts:{DodoServer.SessionManager.GetUsers().Count(user => user.AccessLevel == EUserAccessLevel.Volunteer && !DodoServer.CoordinatorNeedsManager.Data.CurrentNeeds.Any(need => need.Value.ContactedVolunteers.ContainsKey(user.UUID)))}");
				//sab.AppendLine($"Valid contacts:{DodoServer.SessionManager.GetUsers().Where(x}")
				Output(sb.ToString());

			}));
			AddCommand("needs", (async x =>
			{
				foreach (var b in DodoServer.SessionManager.GetUsers())
				{
					b.RequestsSent += DodoServer.CoordinatorNeedsManager.Data.CurrentNeeds.Count(need => need.Value.ContactedVolunteers.ContainsKey(b.UUID));
				}
				var needs = DodoServer.CoordinatorNeedsManager.Data.CurrentNeeds;
				var sb = new StringBuilder("Current Needs:\n");
				foreach(var need in needs)
				{
					var wg = DodoServer.SiteManager.GetWorkingGroup(need.Value.WorkingGroupCode);
					sb.AppendLine($"\t{need.Key}:");
					sb.AppendLine($"\t\tDescription: {need.Value.Description}");
					sb.AppendLine($"\t\tWorking Group: {wg.Name} ({wg.ShortCode}) @ {wg.ParentGroup}");
					sb.AppendLine($"\t\tSite: {need.Value.SiteCode}");
					sb.AppendLine($"\t\tAmount: {Utility.NeedAmountToString(need.Value.Amount)}");
					sb.AppendLine($"\t\tTime Needed: {need.Value.TimeNeeded}");
					sb.AppendLine($"\t\tTime Requested: {need.Value.TimeOfRequest}");
					sb.AppendLine($"\t\tCreator: {need.Value.Creator}");
					sb.AppendLine($"\t\tContacted: {need.Value.ContactedVolunteers.Count}");
					sb.AppendLine($"\t\tConfirmed: {need.Value.ConfirmedVolunteers.Count}");
					sb.AppendLine($"\t\tLast Broadcast: {need.Value.LastBroadcast}");
					sb.AppendLine($"\t\tValid: {DodoServer.SessionManager.GetUsers().Count(user => need.Value.UserIsValidCandidate(user))}");
				}
				Output(sb.ToString());

			}));
			AddCommand("updateneedssheet", (async x =>
			{
				DodoServer.CoordinatorNeedsManager.UpdateNeedsOnGSheet();
			}));
			AddCommand("broadcast", (async x =>
			{
				if (!File.Exists(x))
					return;
				var msg = File.ReadAllText(x);
				Output("Are you really sure?\n" + msg);
				if(Console.ReadLine() == "YES")
				{
					DodoServer.TelegramGateway.Broadcast(new ServerMessage(msg),
						DodoServer.SessionManager.GetUsers().Where(user => user.Active && user.TelegramUser > 0));
				}
			}));
			AddCommand("site", (async x =>
			{
				if (!int.TryParse(x, out var siteNumber) || !DodoServer.SiteManager.IsValidSiteCode(siteNumber))
				{
					Output("Couldn't find site " + siteNumber);
					return;
				}
				if (siteNumber == SiteSpreadsheetManager.OffSiteNumber)
				{
					Output("OFFSITE");
					return;
				}
				var site = DodoServer.SiteManager.GetSite(siteNumber);
				Output($"Site Name: {site.SiteName}\n" +
					site.WorkingGroups.Select(code => DodoServer.SiteManager.GetWorkingGroup(code)).Aggregate("",
					(current, next) => current == "" ? next.ToString() : current.ToString() + "\n" + next.ToString()));
			}));

			var inputThread = new Task(async () =>
			{
				string input = null;
				while(true)
				{
					try
					{
						if(File.Exists(InputPath))
						{
							var allLines = File.ReadAllLines(InputPath);
							File.Delete(InputPath);
							foreach (var line in allLines)
							{
								input = line;
								ProcessCmd(line);
							}
						}
					}
					catch(Exception e)
					{
						Exception(e);
					}
					await Task.Delay(500);
				}
			});
			inputThread.Start();
		}

		private void Exception(Exception e)
		{
			File.WriteAllText(OutputPath, $"{e.Message}");
		}

		private void Output(string text)
		{
			File.WriteAllText(OutputPath, text);
		}

		private void ProcessCmd(string line)
		{
			try
			{
				var split = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				var cmdKey = split.First().ToLowerInvariant();
				if (!m_commands.TryGetValue(cmdKey, out var command))
				{
					throw new Exception("No matching command found for: " + cmdKey);
				}
				var task = new Task(async () =>
				{
					try
					{
						await command.Action(split.Length > 1 ?
							split.Skip(1).Aggregate("", (current, next) => current + " " + next).Trim() : "");
					}
					catch (Exception e)
					{
						Logger.Exception(e);
					}

				});
				task.Start();
			}
			catch(Exception e)
			{
				Logger.Exception(e);
			}
		}

		public void AddCommand(string keyPhrase, Func<string, Task> action)
		{
			m_commands.TryAdd(keyPhrase, new Command()
			{
				Action = action,
			});
		}
	}
}
