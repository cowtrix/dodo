using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public class CoordinatorNeedsManager
	{
		public class Need
		{
			public WorkingGroup WorkingGroup;
			public int SiteCode;
			public int Amount;
			public DateTime TimeNeeded;
			public DateTime TimeOfRequest;

			public Need()
			{
				SiteCode = -1;
				WorkingGroup = default(WorkingGroup);
				Amount = -1;
				TimeOfRequest = default(DateTime);
			}
		}

		private List<Need> m_data;
		private readonly string m_dataOutputSpreadsheetID;
		private bool m_dirty;
		private string m_backupPath;

		public CoordinatorNeedsManager(string dataOutputID)
		{
			m_dataOutputSpreadsheetID = dataOutputID;
			m_backupPath = Path.Combine("Backups", "needs.json");
			LoadFromBackup();
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

			var saveTask = new Task(() =>
			{
				while (true)
				{
					Thread.Sleep(60 * 1000);
					File.WriteAllText(m_backupPath, JsonConvert.SerializeObject(m_data, Formatting.Indented, new JsonSerializerSettings
					{
						TypeNameHandling = TypeNameHandling.Auto
					}));
				}
			});
			saveTask.Start();
		}

		private void LoadFromBackup()
		{
			if(!File.Exists(m_backupPath))
			{
				m_data = new List<Need>();
				return;
			}
			m_data = JsonConvert.DeserializeObject<List<Need>>(File.ReadAllText(m_backupPath));
		}

		public List<Need> GetCurrentNeeds()
		{
			return m_data.ToList();
		}

		public bool AddNeedRequest(User user, Need need)
		{
			if(user.AccessLevel <= EUserAccessLevel.Volunteer)
			{
				return false;
			}
			if(user.SiteCode != 0 && !user.CoordinatorRoles.Any(x => x.SiteCode == need.SiteCode)) // RSO
			{
				return false;
			}
			if(need.Amount == 0)
			{
				return RemoveNeed(need);
			}
			need.TimeOfRequest = DateTime.Now;
			m_data.Add(need);
			m_dirty = true;
			return true;
		}

		public bool RemoveNeed(Need need)
		{
			return m_data.Remove(need);
		}

		void UpdateNeedsOnGSheet()
		{
			m_dirty = false;
			var spreadsheet = new List<List<string>>();
			spreadsheet.Add(new List<string>()
			{
				"Site", "SiteCode", "Parent Group", "Working Group", "Amount Needed", "Contact Code", "Time Needed", "Time Updated"
			});
			var sites = DodoServer.SiteManager.GetSites();
			foreach (var need in m_data)
			{
				var site = sites.First(x => x.SiteCode == need.SiteCode);
				spreadsheet.Add(new List<string>()
				{
					site.SiteName, site.SiteCode.ToString(), need.WorkingGroup.ParentGroup.ToString(), need.WorkingGroup.Name,
					(need.Amount == int.MaxValue  ? "Many" : need.Amount.ToString()), need.WorkingGroup.ShortCode + site.SiteCode.ToString(),
					need.TimeNeeded.ToString(), need.TimeOfRequest.ToString()
				});
			}
			if(!DodoServer.Dummy)
			{
				GSheets.ClearSheet(m_dataOutputSpreadsheetID, "A1:ZZZ");
				GSheets.WriteSheet(m_dataOutputSpreadsheetID, spreadsheet, "A1:ZZZ");
			}
			Logger.Debug($"Updated needs sheet");
		}
	}
}
