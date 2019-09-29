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
			public string Creator;
			public int SiteCode;
			public int Amount;
			public string WorkingGroupCode;
			public string Description;
			public DateTime TimeNeeded;
			public DateTime TimeOfRequest;
			public DateTime LastBroadcast;
			public string Salt;

			public ConcurrentDictionary<string, bool> ContactedVolunteers = new ConcurrentDictionary<string, bool>();
			public ConcurrentDictionary<string, bool> ConfirmedVolunteers = new ConcurrentDictionary<string, bool>();

			[JsonIgnore]
			public string Key { get { return $"{WorkingGroupCode}{SiteCode}{Salt}"; } }
			[JsonIgnore]
			public WorkingGroup WorkingGroup { get { return DodoServer.SiteManager.GetWorkingGroup(WorkingGroupCode); } }
			[JsonIgnore]
			public SiteSpreadsheet Site { get { return DodoServer.SiteManager.GetSite(SiteCode); } }

			public Need() { }

			public Need(User creator, WorkingGroup group, int sitecode, int amount, DateTime timeNeeded, string desc)
			{
				Creator = creator.UUID;
				SiteCode = sitecode;
				WorkingGroupCode = group.ShortCode;
				Amount = amount;
				TimeOfRequest = DateTime.Now;
				TimeNeeded = timeNeeded;
				Description = desc;
			}

			public bool UserIsValidCandidate(User user)
			{
				return user.Active && user.IsVerified() && user.AccessLevel == EUserAccessLevel.Volunteer && user.SiteCode == SiteCode && (TimeNeeded < DodoServer.RebellionStartDate || (user.StartDate < TimeNeeded && user.EndDate >= TimeNeeded));
			}

			public ServerMessage AddConfirmation(User user)
			{
				ConfirmedVolunteers.TryAdd(user.UUID, false);
				DodoServer.DefaultGateway.Broadcast(new ServerMessage($"You've got a new volunteer for role {Key}: {Description}. You can contact them at {user.PhoneNumber}"),
					GetAllCoordinatorContacts());
				Logger.Debug($"User {user} confirmed for role {Key}");
				user.Active = false;
				return new ServerMessage("Great, I've confirmed your availability for this role. A coordinator will be in touch soon.");
			}

			public List<User> GetAllCoordinatorContacts()
			{
				var activeCoordinators = DodoServer.SessionManager.GetUsers()
					.Where(user => user.Active && user.CoordinatorRoles.Any(role => role.SiteCode == SiteCode) &&
					user.CoordinatorRoles.Any(role => role.WorkingGroup.ShortCode == WorkingGroupCode))
					.ToList();
				if (!activeCoordinators.Any(user => user.UUID == Creator))
				{
					var owner = DodoServer.SessionManager.GetUserFromUserID(Creator);
					if (owner != null)
					{
						activeCoordinators.Add(owner);
					}
				}
				return activeCoordinators;
			}
		}

		public class NeedsData
		{
			public ConcurrentDictionary<string, Need> CurrentNeeds = new ConcurrentDictionary<string, Need>();
			public ConcurrentDictionary<string, DateTime> PreviousCodes = new ConcurrentDictionary<string, DateTime>();
		}
		private readonly string m_dataOutputSpreadsheetID;
		private bool m_dirty;
		public const int MaxNeedCountPerWorkingGroup = 8;
		public NeedsData Data = new NeedsData();

		private TimeSpan m_timeout { get { return TimeSpan.FromMinutes(20); } }

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
						if(DodoServer.DefaultGateway != null)
						{
							ProcessNeeds();
						}
						await Task.Delay(TimeSpan.FromSeconds(10));
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

		public void ProcessNeeds()
		{
			if(!Data.CurrentNeeds.Any())
			{
				return;
			}
			List<string> toRemove = new List<string>();
			foreach(var needKey in Data.CurrentNeeds)
			{
				var need = needKey.Value;
				var now = DateTime.Now;
				if(now < need.LastBroadcast + TimeSpan.FromMinutes(10))
				{
					continue;
				}
				need.LastBroadcast = now;
				if(need.ConfirmedVolunteers.Count >= need.Amount)
				{
					// We've got confirmed volunteers, cancel the request
					var contacts = need.GetAllCoordinatorContacts();
					DodoServer.DefaultGateway.Broadcast(
						new ServerMessage($"It looks like you've gotten {Utility.NeedAmountToString(need.Amount)} confirmed volunteers for Volunteer Request {need.Key} ({need.WorkingGroup.Name})." +
						$" This request is now complete and has been removed. You can make a new request at any time with the {CoordinatorNeedsTask.CommandKey} command."), contacts);
					toRemove.Add(need.Key);
					continue;
				}
				if (need.TimeNeeded < now || (need.TimeNeeded == DateTime.MaxValue && now > need.TimeOfRequest + TimeSpan.FromHours(24)))
				{
					// We're past the needed date, cancel the request
					var contacts = need.GetAllCoordinatorContacts();
					DodoServer.DefaultGateway.Broadcast(
						new ServerMessage($"It looks like Volunteer Request {need.Key} ({need.WorkingGroup.Name}) has expired." +
						$" This request has been removed. You can make a new request at any time with the {CoordinatorNeedsTask.CommandKey} command."), contacts);
					toRemove.Add(need.Key);
					continue;
				}
				// Find potentials and notify
				var allUsers = DodoServer.SessionManager.GetUsers();
				var clampedNeed = Math.Min(need.Amount, 20);
				var uncontactedMatchingUsers = allUsers.Where(user => need.UserIsValidCandidate(user) && !need.ConfirmedVolunteers.ContainsKey(user.UUID) && !need.ContactedVolunteers.ContainsKey(user.UUID))	// Get volunteers who will be around at the time and at the site
					.OrderBy(user => user.WorkingGroupPreferences.Contains(need.WorkingGroupCode))	// Put the ones who have selected this working group first
					.ThenBy(user => user.GetTrustScore())
					.Take(Math.Max(0, need.Amount - need.ConfirmedVolunteers.Count)).ToList();
				if(uncontactedMatchingUsers.Count == 0)
				{
					continue;
				}
				DodoServer.DefaultGateway.Broadcast(new ServerMessage($"Hello rebel! It looks like there might be a role needed at your site that you might be able to fill. " +
						(need.Amount == int.MaxValue ? "" : $"There are {need.Amount - need.ConfirmedVolunteers.Count}/{need.Amount} spots still needing to be filled. ")
						+ $"The role is {need.Description} with {need.WorkingGroup.Name}, starting {Utility.ToDateTimeCode(need.TimeNeeded)}. If you can do this, reply {need.Key}."),
						uncontactedMatchingUsers);
				Logger.Debug($"Found {uncontactedMatchingUsers.Count()} new volunteers for need {need.Key}");
				foreach (var volunteer in uncontactedMatchingUsers)
				{
					need.ContactedVolunteers.TryAdd(volunteer.UUID, false);
				}
			}
			foreach(var removeKey in toRemove)
			{
				RemoveNeed(removeKey);
			}
		}

		public void ClearAll()
		{
			Data.CurrentNeeds.Clear();
		}

		public IEnumerable<Need> GetNeedsForWorkingGroup(int sitecode, WorkingGroup group)
		{
			return Data.CurrentNeeds.Values.Where(x => x.WorkingGroupCode == group.ShortCode && x.SiteCode == sitecode);
		}

		public bool AddNeedRequest(User user, WorkingGroup workingGroup, int sitecode, int amount, DateTime timeNeeded, string description)
		{
			return AddNeedRequest(user, workingGroup, sitecode, amount, timeNeeded, description, out _);
		}

		public bool AddNeedRequest(User user, WorkingGroup workingGroup, int sitecode, int amount, DateTime timeNeeded, string description, out string key)
		{
			key = null;
			if (user.AccessLevel <= EUserAccessLevel.Volunteer)
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
			if(currentNeeds > MaxNeedCountPerWorkingGroup)
			{
				return false;
			}
			if(amount == 0)
			{
				return false;
			}
			var newNeed = new Need(user, workingGroup, sitecode, amount, timeNeeded, description);
			do
			{
				newNeed.Salt = Utility.RandomString(2, new Random().Next().ToString());
			}
			while (Data.CurrentNeeds.ContainsKey(newNeed.Key));
			if(!Data.CurrentNeeds.TryAdd(newNeed.Key, newNeed))
			{
				Logger.Alert("Could not add new need with key " + newNeed.Key);
				return false;
			}
			Data.PreviousCodes.TryRemove(newNeed.Key, out _);
			key = newNeed.Key;
			m_dirty = true;
			return true;
		}

		public bool RemoveNeed(Need need)
		{
			m_dirty = true;
			Data.PreviousCodes.TryAdd(need.Key, DateTime.Now);
			return Data.CurrentNeeds.TryRemove(need.Key, out _);
		}

		public bool RemoveNeed(string needKey)
		{
			m_dirty = true;
			return Data.CurrentNeeds.TryRemove(needKey, out _);
		}

		public void UpdateNeedsOnGSheet()
		{
			m_dirty = false;
			var spreadsheet = new List<List<string>>();
			spreadsheet.Add(new List<string>()
			{
				"Site", "SiteCode", "Parent Group", "Working Group", "Amount Needed", "Role Description", "Contact Code", "Time Needed", "Time Updated", "Volunteers Confirmed"
			});
			var sites = DodoServer.SiteManager.GetSites();
			foreach (var needKey in Data.CurrentNeeds.OrderBy(x => x.Value.TimeOfRequest))
			{
				var need = needKey.Value;
				var site = sites.First(x => x.SiteCode == need.SiteCode);
				spreadsheet.Add(new List<string>()
				{
					site.SiteName, site.SiteCode.ToString(), need.WorkingGroup.ParentGroup.ToString(), need.WorkingGroup.Name,
					(need.Amount == int.MaxValue  ? "MANY" : need.Amount.ToString()), need.Description, need.Key,
					(need.TimeNeeded == DateTime.MaxValue ? "NOW" : need.TimeNeeded.ToString()), need.TimeOfRequest.ToString(), need.ConfirmedVolunteers.Count.ToString(),
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
			File.WriteAllText(dataPath, JsonConvert.SerializeObject(Data, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
			//Logger.Debug($"Saved user session data to {dataPath}");
		}

		public void LoadFromFile(string backupFolder)
		{
			var backupPath = Path.Combine(backupFolder, "needs.json");
			if (DodoServer.Dummy || DodoServer.NoLoad || !File.Exists(backupPath))
			{
				Data = new NeedsData();
				return;
			}
			Data = JsonConvert.DeserializeObject<NeedsData> (File.ReadAllText(backupPath), new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
			Logger.Debug($"Loaded {Data.CurrentNeeds.Count} Volunteer Requests from " + backupPath);
		}
	}
}
