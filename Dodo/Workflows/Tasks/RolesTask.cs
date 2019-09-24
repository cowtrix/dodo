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
			var cmd = message.ContentUpper.FirstOrDefault();
			if (message.ContentUpper.FirstOrDefault() == "DONE")
			{
				response = ExitTask(session);
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
				response = new ServerMessage((cmd == CommandKey ? "" : "Sorry, I didn't understand that selection. ") + 
					GetParentGroupSelectionString());
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
						addedOrRemoved = "Okay, I removed that. ";
						user.WorkingGroupPreferences.Remove(workingGroup.ShortCode);
					}
					else
					{
						addedOrRemoved = "Okay, I added that. ";
						user.WorkingGroupPreferences.Add(workingGroup.ShortCode);
					}
					response = new ServerMessage(addedOrRemoved + GetWorkingGroupList(user, m_parentGroupFilter.Value));
					return true;
				}
			}
			response = default;
			return false;
		}

		private string GetWorkingGroupList(User user, EParentGroup pg)
		{
			var site = user.Site;
			var wgs = DodoServer.SiteManager.Data.WorkingGroups.Where(x => x.Value.ParentGroup == pg &&
				(site == null || site.WorkingGroups.Contains(x.Key)));
			if(wgs.Count() == 0)
			{
				m_parentGroupFilter = null;
				return $"Sorry, it doesn't look like there are any coordinators for this parent group yet at site {site.SiteName}."
					+ GetParentGroupSelectionString();
			}
			var sb = new StringBuilder("Select a Working Group to add it to your preferences:\n");
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
