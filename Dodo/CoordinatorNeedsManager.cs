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
			public Dictionary<string, bool> PotentialVolunteers = new Dictionary<string, bool>();

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

		public ConcurrentDictionary<string, Need> CurrentNeeds;
		private readonly string m_dataOutputSpreadsheetID;
		private bool m_dirty;
		public const int MaxNeedCount = 5;

		public CoordinatorNeedsManager(Configuration config)
		{
			m_dataOutputSpreadsheetID = config.SpreadsheetData.CoordinatorNeedsSpreadsheetID;
			LoadFromFile(config.BackupPath);
			var updateTask = new Task(async () =>
			{
				while (true)
				{
					await Task.Delay(10 * 1000);
					if(m_dirty)
						UpdateNeedsOnGSheet();
				}
			});
			var matchMakerTask = new Task(async () =>
			{
				while(true)
				{
					try
					{
						await DoMatchmakingHunt();
						await Task.Delay(500);
					}
					catch(Exception e)
					{
						Logger.Exception(e, "Exception in matchmaker");
					}
				}
			});
			matchMakerTask.Start();
			if (!DodoServer.Dummy)
			{
				updateTask.Start();
			}
		}

		private async Task DoMatchmakingHunt()
		{
			if(!CurrentNeeds.Any())
			{
				return;
			}
			foreach(var needKey in CurrentNeeds)
			{
				var need = needKey.Value;
				if(need.PotentialVolunteers.Count(x => x.Value) >= need.Amount)
				{
					// We've got confirmed volunteers
					// TODO End the task?
					continue;
				}
				var matchingUsers = DodoServer.SessionManager.GetUsers()
					.Where(x => x.StartDate < need.TimeNeeded && x.EndDate > need.TimeNeeded)
					.OrderBy(x => x.WorkingGroupPreferences.Contains(need.WorkingGroup.ShortCode));
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
			if(!CurrentNeeds.TryAdd(newNeed.Key, newNeed))
			{
				Logger.Alert("Could not add new need with key " + newNeed.Key);
				return false;
			}
			m_dirty = true;
			return true;
		}

		public bool RemoveNeed(Need need)
		{
			m_dirty = true;
			return CurrentNeeds.TryRemove(need.Key, out _);
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
			if(DodoServer.Dummy || DodoServer.NoLoad)
			{
				Logger.Debug("Skipped save in Needs Manager");
				return;
			}
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
			if (DodoServer.Dummy || DodoServer.NoLoad || !File.Exists(backupPath))
			{
				CurrentNeeds = new ConcurrentDictionary<string, Need>();
				return;
			}
			CurrentNeeds = JsonConvert.DeserializeObject<ConcurrentDictionary<string, Need>> (File.ReadAllText(backupPath), new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			Logger.Debug("Loaded needs data from " + backupPath);
		}
	}
}
