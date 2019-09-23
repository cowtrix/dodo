using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;

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
			public string Salt;
			public string Description;

			[JsonIgnore]
			public string Key { get { return $"{WorkingGroup.ShortCode}{SiteCode}{Salt}"; } }

			public Need() { }

			public Need(WorkingGroup group, int sitecode, int amount, DateTime timeNeeded, string desc)
			{
				SiteCode = sitecode;
				WorkingGroup = group;
				Amount = amount;
				TimeOfRequest = DateTime.Now;
				TimeNeeded = timeNeeded;
				Description = desc;
			}
		}

		public Dictionary<string,Need> CurrentNeeds;
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

		public IEnumerable<Need> GetNeedsForWorkingGroup(int sitecode, WorkingGroup group)
		{
			return CurrentNeeds.Values.Where(x => x.WorkingGroup.Equals(group) && x.SiteCode == sitecode);
		}

		public List<Need> GetCurrentNeeds()
		{
			return CurrentNeeds.Values.ToList();
		}

		public bool AddNeedRequest(User user, WorkingGroup workingGroup, int sitecode, int amount, DateTime timeNeeded, string description)
		{
			if(user.AccessLevel <= EUserAccessLevel.Volunteer)
			{
				return false;
			}
			if (user.AccessLevel <= EUserAccessLevel.RotaCoordinator && !user.CoordinatorRoles.Any(x => x.SiteCode == sitecode)) // RSO
			{
				if (user.AccessLevel <= EUserAccessLevel.Coordinator && !user.CoordinatorRoles.Any(x => x.WorkingGroup.ShortCode == workingGroup.ShortCode)) // RSO
				{
					return false;
				}
				return false;
			}
			var currentNeeds = GetNeedsForWorkingGroup(sitecode, workingGroup).Count();
			if(currentNeeds > MaxNeedCount)
			{
				return false;
			}
			if(amount == 0)
			{
				return false;
			}

			var newNeed = new Need(workingGroup, sitecode, amount, timeNeeded, description);
			do
			{
				newNeed.Salt = Utility.RandomString(2, new Random().Next().ToString());
			}
			while (CurrentNeeds.ContainsKey(newNeed.Key));
			CurrentNeeds.Add(newNeed.Key, newNeed);
			m_dirty = true;
			return true;
		}

		public bool RemoveNeed(Need need)
		{
			m_dirty = true;
			return CurrentNeeds.Remove(need.Key);
		}

		void UpdateNeedsOnGSheet()
		{
			m_dirty = false;
			var spreadsheet = new List<List<string>>();
			spreadsheet.Add(new List<string>()
			{
				"Site", "SiteCode", "Parent Group", "Working Group", "Amount Needed", "Description", "Contact Code", "Time Needed", "Time Updated"
			});
			var sites = DodoServer.SiteManager.GetSites();
			foreach (var needKey in CurrentNeeds.OrderBy(x => x.Value.TimeOfRequest))
			{
				var need = needKey.Value;
				var site = sites.First(x => x.SiteCode == need.SiteCode);
				spreadsheet.Add(new List<string>()
				{
					site.SiteName, site.SiteCode.ToString(), need.WorkingGroup.ParentGroup.ToString(), need.WorkingGroup.Name,
					(need.Amount == int.MaxValue  ? "Many" : need.Amount.ToString()), need.Description, need.Key,
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
				CurrentNeeds = new Dictionary<string, Need>();
				return;
			}
			CurrentNeeds = JsonConvert.DeserializeObject<Dictionary<string, Need>> (File.ReadAllText(backupPath), new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			Logger.Debug("Loaded needs data from " + backupPath);
		}
	}
}
