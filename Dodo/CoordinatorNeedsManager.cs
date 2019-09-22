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
	public class CoordinatorNeedsManager : IBackup
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

		public List<Need> CurrentNeeds;
		private readonly string m_dataOutputSpreadsheetID;
		private bool m_dirty;
		public const int MaxNeedCount = 5;

		public CoordinatorNeedsManager(Configuration config)
		{
			m_dataOutputSpreadsheetID = config.SpreadsheetData.CoordinatorNeedsSpreadsheetID;
			LoadFromFile(config.BackupPath);
			var updateTask = new Task(() =>
			{
				while (true)
				{
					Thread.Sleep(10 * 1000);
					if(m_dirty)
						UpdateNeedsOnGSheet();
				}
			});
			if(!DodoServer.Dummy)
			{
				updateTask.Start();
			}
		}

		public void ClearAll()
		{
			CurrentNeeds.Clear();
		}

		public IEnumerable<Need> GetNeedsForWorkingGroup(WorkingGroup group)
		{
			return CurrentNeeds.Where(x => x.WorkingGroup.Equals(group));
		}

		public List<Need> GetCurrentNeeds()
		{
			return CurrentNeeds.ToList();
		}

		public bool AddNeedRequest(User user, Need need)
		{
			if(user.AccessLevel <= EUserAccessLevel.Volunteer)
			{
				return false;
			}
			if (user.AccessLevel <= EUserAccessLevel.RotaCoordinator && !user.CoordinatorRoles.Any(x => x.SiteCode == need.SiteCode)) // RSO
			{
				if (user.AccessLevel <= EUserAccessLevel.Coordinator && !user.CoordinatorRoles.Any(x => x.WorkingGroup.ShortCode == need.WorkingGroup.ShortCode)) // RSO
				{
					return false;
				}
				return false;
			}
			var currentNeeds = GetNeedsForWorkingGroup(need.WorkingGroup).Count();
			if(currentNeeds > MaxNeedCount)
			{
				return false;
			}
			if(need.Amount == 0)
			{
				return RemoveNeed(need);
			}
			need.TimeOfRequest = DateTime.Now;
			CurrentNeeds.Add(need);
			m_dirty = true;
			return true;
		}

		public bool RemoveNeed(Need need)
		{
			return CurrentNeeds.Remove(need);
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
			foreach (var need in CurrentNeeds)
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

		public void SaveToFile(string backupFolder)
		{
			var dataPath = Path.Combine(backupFolder, "needs.json");
			File.WriteAllText(dataPath, JsonConvert.SerializeObject(CurrentNeeds, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
			Logger.Debug($"Saved user session data to {dataPath}");
		}

		public void LoadFromFile(string backupFolder)
		{
			var backupPath = Path.Combine(backupFolder, "needs.json");
			if (!File.Exists(backupPath))
			{
				CurrentNeeds = new List<Need>();
				return;
			}
			CurrentNeeds = JsonConvert.DeserializeObject<List<Need>>(File.ReadAllText(backupPath), new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			Logger.Debug("Loaded needs data from " + backupPath);
		}
	}
}
