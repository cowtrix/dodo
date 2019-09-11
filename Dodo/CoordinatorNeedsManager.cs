using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public class CoordinatorNeedsManager
	{
		public struct Need
		{
			public WorkingGroup WorkingGroup;
			public int SiteCode;
			public int Amount;
			public DateTime TimeOfRequest;
		}
		private Dictionary<WorkingGroup, Need> m_data = new Dictionary<WorkingGroup, Need>();
		private readonly string m_dataOutputSpreadsheetID;
		private bool m_dirty;
		public CoordinatorNeedsManager(string dataOutputID)
		{
			m_dataOutputSpreadsheetID = dataOutputID;
			ReadFromGSheets();
			var updateTask = new Task(() =>
			{
				while (true)
				{
					Thread.Sleep(10 * 1000);
					if(m_dirty)
						UpdateNeedsOnGSheet();
				}
			});
			updateTask.Start();
		}

		void ReadFromGSheets()
		{
			var data = GSheets.GetSheet(m_dataOutputSpreadsheetID);

		}

		public bool AddNeedRequest(User user, Need need)
		{
			if(!user.IsCoordinator)
			{
				return false;
			}
			if(user.SiteCode != 0 && !user.CoordinatorRoles.Any(x => x.SiteCode == need.SiteCode)) // RSO
			{
				return false;
			}
			if(need.Amount == 0)
			{
				m_dirty = true;
				return m_data.Remove(need.WorkingGroup);
			}
			need.TimeOfRequest = DateTime.Now;
			m_data[need.WorkingGroup] = need;
			m_dirty = true;
			return true;
		}

		void UpdateNeedsOnGSheet()
		{
			m_dirty = false;
			var spreadsheet = new List<List<string>>();
			spreadsheet.Add(new List<string>()
			{
				"Site", "SiteCode", "Parent Group", "Working Group", "Amount Needed", "Contact Code", "Time Updated"
			});
			var sites = DodoServer.SiteManager.GetSites();
			foreach (var need in m_data)
			{
				var site = sites.First(x => x.SiteCode == need.Key.SiteCode);
				spreadsheet.Add(new List<string>()
				{
					site.SiteName, site.SiteCode.ToString(), need.Key.ParentGroup.ToString(), need.Key.Name,
					need.Value.Amount.ToString(), need.Key.ShortCode, need.Value.TimeOfRequest.ToString()
				});
			}
			GSheets.ClearSheet(m_dataOutputSpreadsheetID, "A1:ZZZ");
			GSheets.WriteSheet(m_dataOutputSpreadsheetID, spreadsheet, "A1:ZZZ");
			Console.WriteLine($"Updated needs sheet");
		}
	}
}
