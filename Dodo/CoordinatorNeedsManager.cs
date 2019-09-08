using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public class CoordinatorNeedsManager
	{
		public Dictionary<WorkingGroup, int> _volunteersNeeded = new Dictionary<WorkingGroup, int>();
		private readonly string m_dataOutputSpreadsheetID;
		public CoordinatorNeedsManager(string dataOutputID)
		{
			m_dataOutputSpreadsheetID = dataOutputID;
			ReadFromGSheets();
			var updateTask = new Task(() =>
			{
				while (true)
				{
					Thread.Sleep(10 * 1000);
					UpdateNeedsOnGSheet();
				}
			});
			updateTask.Start();
		}

		void ReadFromGSheets()
		{
			var data = GSheets.GetSheet(m_dataOutputSpreadsheetID);

		}

		void UpdateNeedsOnGSheet()
		{
			var data = new List<List<string>>();
			data.Add(new List<string>()
			{
				"Site", "SiteCode", "Working Group"
			});
			foreach (var site in DodoServer.SiteManager.GetSites())
			{
				foreach (var wg in Enum.GetValues(typeof(EParentGroup)))
				{
					data.Add(new List<string>()
					{
						site.SiteName, site.SiteCode.ToString("00"), wg.ToString(),
					});
				}
			}
			GSheets.WriteSheet<string>(m_dataOutputSpreadsheetID, data, "A1:ZZZ");
		}
	}
}
