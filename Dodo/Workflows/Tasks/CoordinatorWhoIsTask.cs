using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	[WorkflowTaskInfo(EUserAccessLevel.Coordinator)]
	public class CoordinatorWhoIsTask : WorkflowTask
	{
		public static string CommandKey { get { return "WHOIS"; } }
		public static string HelpString { get { return $"{CommandKey} - find out contact details for coordinators of certain Working Groups."; } }

		public string ShortCode;
		bool m_listMode;
		EParentGroup? m_parentGroupFilter;

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var toUpper = message.Content.ToUpperInvariant()
				.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < toUpper.Length; i++)
			{
				string cmd = toUpper[i];
				if (cmd == CommandKey)
				{
					if (i >= toUpper.Length - 1)
					{
						response = new ServerMessage("Please tell me the Working Group Code you would like." +
							" If you aren't sure what the code is and want to see a list, reply LIST");
						return true;
					}
					continue;
				}
				if (DodoServer.SiteManager.IsValidWorkingGroup(cmd))
				{
					var wg = DodoServer.SiteManager.GetWorkingGroup(cmd);
					var all = DodoServer.SessionManager.GetUsers().Where(x => x.Active && x.CoordinatorRoles.Any(y => y.WorkingGroup.ShortCode == cmd));
					ExitTask(session);
					response = new ServerMessage(all.Aggregate($"Coordinators for {wg.Name}:", (current, next) => current + "\n"
						+ $"{next.Name} - Site: {next.CoordinatorRoles.First(x => x.WorkingGroup.ShortCode == cmd).Site.SiteName}, Ph: {next.PhoneNumber ?? "None"}, Email: {next.Email}"));
					return true;
				}
				if (cmd == "LIST")
				{
					m_listMode = true;
					if(i >= toUpper.Length - 1)
					{
						var sb = new StringBuilder("Please select the parent group:\n");
						foreach(var parentGroup in Enum.GetValues(typeof(EParentGroup)).OfType<EParentGroup>())
						{
							sb.AppendLine($"{(int)parentGroup} - {parentGroup.GetName()}");
						}
						response = new ServerMessage(sb.ToString());
						return true;
					}
					continue;
				}
				if(m_listMode)
				{
					if(m_parentGroupFilter == null)
					{
						if (!int.TryParse(cmd, out var parentGroup) || parentGroup < 0 || parentGroup > (int)EParentGroup.RSO)
						{
							var stb = new StringBuilder("Sorry, that didn't seem like a valid choice. Please select the parent group:\n");
							foreach (var pg in Enum.GetValues(typeof(EParentGroup)).OfType<EParentGroup>())
							{
								stb.AppendLine($"{(int)pg} - {pg.GetName()}");
							}
							stb.AppendLine("Or, if you want to cancel, reply DONE");
							response = new ServerMessage(stb.ToString());
							return true;
						}
						m_parentGroupFilter = (EParentGroup)parentGroup;
						var wgs = DodoServer.SiteManager.Data.WorkingGroups.Where(x => x.Value.ParentGroup == m_parentGroupFilter.Value);
						var sb = new StringBuilder("Please select the working group:\n");
						foreach(var wg in wgs)
						{
							sb.AppendLine($"{wg.Value.ShortCode} - {wg.Value.Name}");
						}
						response = new ServerMessage(sb.ToString());
						return true;
					}
				}
			}
			response = default;
			return false;
		}
	}
}
