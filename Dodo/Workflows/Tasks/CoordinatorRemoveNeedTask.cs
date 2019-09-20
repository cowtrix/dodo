using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XR.Dodo
{
	public class CoordinatorRemoveNeedTask : WorkflowTask
	{
		public CoordinatorNeedsManager.Need Need = null;

		public static string CommandKey { get { return "DELETENEED"; } }
		public static string HelpString { get { return $"{CommandKey} - delete a Volunteer Request that you or someone from your working group has made."; } }

		public CoordinatorRemoveNeedTask(Workflow workflow) : base(workflow)
		{
		}

		List<CoordinatorNeedsManager.Need> GetNeeds(User user)
		{
			var needs = DodoServer.CoordinatorNeedsManager.GetCurrentNeeds();
			if (user.AccessLevel == EUserAccessLevel.RSO)
			{
				return needs;
			}
			if(user.AccessLevel == EUserAccessLevel.RotaCoordinator)
			{
				return needs.Where(x => user.CoordinatorRoles.Any(y => y.SiteCode == x.SiteCode)).ToList();
			}
			if (user.AccessLevel == EUserAccessLevel.Coordinator)
			{
				return needs.Where(x => user.CoordinatorRoles.Any(y => y.WorkingGroup.ShortCode == x.WorkingGroup.ShortCode)).ToList();
			}
			throw new Exception("Bad user: " + user.UUID);
		}

		public override bool ProcessMessage(UserMessage message, UserSession session, out ServerMessage response)
		{
			var toUpper = message.Content.ToUpperInvariant()
				.Split(new[] { " " }, System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < toUpper.Length; i++)
			{
				string cmd = (string)toUpper[i];
				if (cmd == "CANCEL")
				{
					ExitTask();
					response = new ServerMessage("Okay, I've canceled this request.");
					return true;
				}

				var user = session.GetUser();
				if(Need == null)
				{
					var needs = GetNeeds(user);
					if (int.TryParse(cmd, out var pick) && pick >= 0 && pick < needs.Count)
					{
						Need = needs[pick];
						response = Finalize();
						return true;
					}
					if(needs.Count == 1)
					{
						Need = needs.First();
						response = Finalize();
						return true;
					}
					response = GetNeedsMenu(needs);
					return true;
				}

			}
			response = default;
			return false;
		}

		private ServerMessage Finalize()
		{
			ExitTask();
			if (DodoServer.CoordinatorNeedsManager.RemoveNeed(Need))
				return new ServerMessage("Great, I've removed that Volunteer Request from the system.");
			else
				return new ServerMessage("Sorry, that didn't work for some reason. Please try again.");
		}

		private ServerMessage GetNeedsMenu(List<CoordinatorNeedsManager.Need> needs)
		{
			var sb = new StringBuilder("Please select a Volunteer Request to cancel:\n");
			var needIter = needs.OrderBy(x => x.SiteCode).ToList();
			var siteCode = needIter.First().SiteCode;
			for (int i = 0; i < needIter.Count; i++)
			{
				CoordinatorNeedsManager.Need need = needIter[i];
				if (siteCode != need.SiteCode)
				{
					siteCode = need.SiteCode;
					sb.AppendLine($"Site {siteCode}:");
				}
				sb.AppendLine($"{i} - {need.WorkingGroup.Name} - {need.Amount} for {need.TimeNeeded.ToString("dd/MM HH:mm")}");
			}
			return new ServerMessage(sb.ToString());
		}
	}
}
