using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	public class CoordinatorWhoIsTask : WorkflowTask
	{
		public static string CommandKey { get { return "WHOIS"; } }

		public CoordinatorWhoIsTask(Workflow workflow) : base(workflow)
		{
		}

		public string ShortCode;

		public override ServerMessage ProcessMessage(UserMessage message, UserSession session)
		{
			var toUpper = message.Content.ToUpperInvariant()
				.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < toUpper.Length; i++)
			{
				string cmd = toUpper[i];
				if (cmd == "CANCEL")
				{
					ExitTask();
					return new ServerMessage("Okay, I've canceled this request.");
				}
				if(cmd == CommandKey || !DodoServer.SiteManager.IsValidWorkingGroup(cmd))
				{
					if(i >= toUpper.Length - 1)
						return new ServerMessage("Please tell me the Working Group Code you would like." +
							" If you aren't sure what the code is and want to see a list, reply LIST");
					continue;
				}
				else
				{
					var wg = DodoServer.SiteManager.GetWorkingGroup(cmd);
					var all = DodoServer.SessionManager.GetUsers().Where(x => x.CoordinatorRoles.Any(y => y.WorkingGroup.ShortCode == cmd));
					ExitTask();
					return new ServerMessage(all.Aggregate($"Coordinators for {wg.Name}:", (current, next) => current + "\n"
						+ $"{next.Name} - Site: {next.CoordinatorRoles.First(x => x.WorkingGroup.ShortCode == cmd).Site.SiteName}, Ph: {next.PhoneNumber ?? "None"}, Email: {next.Email}"));
				}
			}
			return default;
		}
	}
}
