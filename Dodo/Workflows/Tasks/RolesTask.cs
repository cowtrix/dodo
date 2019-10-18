using System;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	[WorkflowTaskInfo(EUserAccessLevel.Volunteer)]
	public class RolesTask : WorkflowTask
	{
		public static string CommandKey { get { return "ROLES"; } }
		public static string HelpString { get { return $"{CommandKey} - change what groups you want to volunteer for."; } }

		public EParentGroup? m_parentGroupFilter;
		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var user = session.GetUser();
			for (var i = 0; i < message.ContentUpper.Length; ++i)
			{
				var cmd = message.ContentUpper[i];
				if (message.ContentUpper.FirstOrDefault() == "DONE")
				{
					response = ExitTask(session);
					return true;
				}
				if (m_parentGroupFilter == null && !DodoServer.SiteManager.IsValidWorkingGroup(cmd))
				{
					if (int.TryParse(cmd, out var pGroup) && pGroup >= 0 && pGroup <= (int)EParentGroup.RSO)
					{
						m_parentGroupFilter = (EParentGroup)pGroup;
						if (i < message.ContentUpper.Length - 1)
						{
							continue;
						}
						response = new ServerMessage(GetWorkingGroupList(user, m_parentGroupFilter.Value));
						return true;
					}
					// Get parent group
					if (i < message.ContentUpper.Length - 1)
					{
						continue;
					}
					response = new ServerMessage((cmd == CommandKey ? "Okay, tell me which Working Groups you'd like to volunteer for. " +
						$"If you aren't sure what working groups are, or what a specific one does, go here: {DodoServer.RolesSiteURL} for more information.\n" : "Sorry, I didn't understand that selection. ") +
						GetParentGroupSelectionString());
					return true;
				}
				else
				{
					if (cmd == "BACK")
					{
						response = new ServerMessage(GetParentGroupSelectionString());
						m_parentGroupFilter = null;
						return true;
					}
					if (DodoServer.SiteManager.IsValidWorkingGroup(cmd, out var workingGroup))
					{
						string addedOrRemoved;
						if (user.WorkingGroupPreferences.Contains(workingGroup.ShortCode))
						{
							addedOrRemoved = $"Okay, I removed {workingGroup.Name} from your volunteering preferences. ";
							user.WorkingGroupPreferences.Remove(workingGroup.ShortCode);
						}
						else
						{
							addedOrRemoved = $"Okay, I added {workingGroup.Name} to your volunteering preferences. ";
							user.WorkingGroupPreferences.Add(workingGroup.ShortCode);
						}
						if (i < message.ContentUpper.Length - 1)
						{
							continue;
						}
						if(m_parentGroupFilter == null)
						{
							response = new ServerMessage(addedOrRemoved + " If you didn't mean to do this, just repeat the last command.");
							ExitTask(session);
							return true;
						}
						response = new ServerMessage(addedOrRemoved + GetWorkingGroupList(user, m_parentGroupFilter.Value));
						return true;
					}
				}
			}
			response = default;
			return false;
		}

		private string GetWorkingGroupList(User user, EParentGroup pg)
		{
			var site = user.Site;
			var wgs = DodoServer.SiteManager.Data.WorkingGroups.Where(x => x.Value.ParentGroup == pg);
			if(wgs.Count() == 0)
			{
				m_parentGroupFilter = null;
				return $"Sorry, it doesn't look like there are any coordinators for this parent group yet at site {site?.SiteName}."
					+ GetParentGroupSelectionString();
			}
			var sb = new StringBuilder("Select a Working Group to add or remove it from your preferences:\n");
			foreach(var wg in wgs)
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

		public override ServerMessage ExitTask(UserSession session)
		{
			base.ExitTask(session);
			return new ServerMessage("Okay, I've updated your preferences.");
		}
	}
}
