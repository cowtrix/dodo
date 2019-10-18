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
			public WorkingGroup WorkingGroup { get {
					if(!DodoServer.SiteManager.IsValidWorkingGroup(WorkingGroupCode))
					{
						return new WorkingGroup();
					}
					return DodoServer.SiteManager.GetWorkingGroup(WorkingGroupCode); } }
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
				return user.Active && user.IsVerified() && user.AccessLevel == EUserAccessLevel.Volunteer 
					&& user.StartDate < TimeNeeded && user.EndDate >= TimeNeeded;
			}

			public ServerMessage AddConfirmation(User user)
			{
				ConfirmedVolunteers.TryAdd(user.UUID, false);
				DodoServer.DefaultGateway.Broadcast(new ServerMessage($"You've got a new volunteer for role {Key}: {Description}.\n" +
					$"Name: {user.Name}\nNumber: {user.PhoneNumber}{(string.IsNullOrEmpty(user.Email) ? "" : $"Email: {user.Email}")}\n" +
					"It's now up to you to contact this volunteer and integrate them into the role. If you need help doing that, talk to the Integration Team at your site."),
					GetAllCoordinatorContacts());
				Logger.Debug($"User {user} confirmed for role {Key}");
				user.Active = false;
				user.Karma += 500;
				DodoServer.CoordinatorNeedsManager.Data.TotalRequestsAccepted++;
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
		public class Status
		{
			public string WorkingGroupCode;
			public int SiteCode;
			public int Stressed;
			public int Understaffed;
			public int Unsafe;
			public int VolunteerCount;
			public DateTime TimeSubmitted;
		}

		public class NeedsData
		{
			public int TotalRequestsSent;
			public int TotalRequestsAccepted;
			public int VolunteerRequestsCompleted;
			public ConcurrentDictionary<string, DateTime> LastContacted = new ConcurrentDictionary<string, DateTime>();
			public ConcurrentDictionary<string, Need> CurrentNeeds = new ConcurrentDictionary<string, Need>();
			public ConcurrentDictionary<string, Status> Checkins = new ConcurrentDictionary<string, Status>();
			public ConcurrentDictionary<string, DateTime> CheckinReminders = new ConcurrentDictionary<string, DateTime>();
			public ConcurrentDictionary<string, DateTime> PreviousCodes = new ConcurrentDictionary<string, DateTime>();
		}

		private readonly string m_dataOutputSpreadsheetID;
		private bool m_dirty;
		public const int MaxNeedCountPerWorkingGroup = 8;
		public NeedsData Data = new NeedsData();

		private TimeSpan m_timeout { get { return TimeSpan.FromMinutes(20); } }

		public void AddCheckin(User user, int stress, int understaffed, int safety, int volunteerCount)
		{
			Data.CheckinReminders.TryRemove(user.UUID, out _);
			var siteCode = user.SiteCode;
			if (user.AccessLevel > EUserAccessLevel.Volunteer)
			{
				siteCode = user.CoordinatorRoles.FirstOrDefault().SiteCode;
			}
			var checkin = new Status()
			{
				WorkingGroupCode = user.CoordinatorRoles.FirstOrDefault()?.WorkingGroupCode,
				SiteCode = siteCode,
				Stressed = stress,
				Understaffed = understaffed,
				Unsafe = safety,
				VolunteerCount = volunteerCount,
				TimeSubmitted = DateTime.Now,
			};
			Data.Checkins[user.UUID] = checkin;
			m_dirty = true;
		}

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
							await ProcessNeeds();
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

		public async Task ProcessNeeds()
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
				var timeSinceCreation = now - need.TimeOfRequest;
				var timeout = TimeSpan.FromMinutes(10);
				if(now < need.LastBroadcast + timeout)
				{
					continue;
				}
				if(timeSinceCreation < TimeSpan.FromHours(1) && (now.Hour < 8 || now.Hour > 22))
				{
					continue;
				}
				need.LastBroadcast = now;
				if(need.ConfirmedVolunteers.Count >= need.Amount)
				{
					// We've got confirmed volunteers, cancel the request
					var contacts = need.GetAllCoordinatorContacts();
					DodoServer.DefaultGateway.Broadcast(
						new ServerMessage($"It looks like you've gotten {Utility.NeedAmountToString(need.Amount)} confirmed volunteers for Volunteer Request {need.Key} ({need.WorkingGroup.Name}) - {need.Description}." +
						$" This request is now complete and has been removed. You can make a new request at any time with the {CoordinatorNeedsTask.CommandKey} command."), contacts);
					toRemove.Add(need.Key);
					Data.VolunteerRequestsCompleted++;
					continue;
				}
				if (need.TimeNeeded < now || (now >= DodoServer.RebellionStartDate && (need.TimeNeeded == DateTime.MaxValue && now > need.TimeOfRequest + TimeSpan.FromHours(24))))
				{
					// We're past the needed date, cancel the request
					var contacts = need.GetAllCoordinatorContacts();
					DodoServer.DefaultGateway.Broadcast(
						new ServerMessage($"It looks like Volunteer Request {need.Key} ({need.WorkingGroup.Name}) - {need.Description} has expired." +
						$" This request has been removed. You can make a new request at any time with the {CoordinatorNeedsTask.CommandKey} command."), contacts);
					toRemove.Add(need.Key);
					Data.VolunteerRequestsCompleted++;
					continue;
				}
				// Find potentials and notify
				var allUsers = DodoServer.SessionManager.GetUsers();
				var clampedNeed = Math.Min(need.Amount, 100);
				var uncontactedMatchingUsers = allUsers.Where(user => 
					need.UserIsValidCandidate(user) && 
					!need.ConfirmedVolunteers.ContainsKey(user.UUID) && 
					!need.ContactedVolunteers.ContainsKey(user.UUID) &&
					//user.RequestsSent < 10 &&
					!(DodoServer.SessionManager.GetOrCreateSession(user).Workflow.CurrentTask is IntroductionTask) &&
					(user.SpamSetting != User.ESpamSetting.LESS || user.WorkingGroupPreferences.Contains(need.WorkingGroupCode)))	// Get volunteers who will be around at the time and at the site
					//.OrderBy(user => user.RequestsSent)    // Put the ones who have selected this working group first
					.OrderBy(user => user.WorkingGroupPreferences.Contains(need.WorkingGroupCode))
					
					.ThenByDescending(user => user.SiteCode == need.SiteCode)
					.ThenByDescending(user => user.SpamSetting == User.ESpamSetting.MORE)
					.ThenBy(user => user.RequestsSent)
					.Take(Math.Max(0, (clampedNeed - need.ConfirmedVolunteers.Count)) * 2).ToList();
				if(uncontactedMatchingUsers.Count == 0)
				{
					continue;
				}
				DodoServer.DefaultGateway.Broadcast(new ServerMessage($"Hello rebel! There's a new Volunteer Request you might be interested in. " +
						(need.Amount == int.MaxValue ? "" : $"There are {need.Amount - need.ConfirmedVolunteers.Count}/{need.Amount} spots still needing to be filled. ")
						+ $"The role is {need.Description} with {need.WorkingGroup.Name}{(need.Site != null ? $" at site {need.Site.SiteName}" : "")}, starting {Utility.ToDateTimeCode(need.TimeNeeded)}. If this sounds like something you might be interested in, reply {need.Key}"),
						uncontactedMatchingUsers);
				Logger.Debug($"Found {uncontactedMatchingUsers.Count()} new volunteers for need {need.Key}");
				Data.TotalRequestsSent += uncontactedMatchingUsers.Count();
				foreach (var volunteer in uncontactedMatchingUsers)
				{
					need.ContactedVolunteers.TryAdd(volunteer.UUID, false);
					Data.LastContacted[volunteer.UUID] = now;
					volunteer.RequestsSent++;
				}
				await Task.Delay(TimeSpan.FromMinutes(1));
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
			return RemoveNeed(need.Key);
		}

		public bool RemoveNeed(string needKey)
		{
			m_dirty = true;
			Data.PreviousCodes.TryAdd(needKey, DateTime.Now);
			return Data.CurrentNeeds.TryRemove(needKey, out _);
		}

		public void UpdateNeedsOnGSheet()
		{
			m_dirty = false;

			var needs = new List<List<string>>();
			needs.Add(new List<string>()
			{
				"Site", "SiteCode", "Parent Group", "Working Group", "Amount Needed", "Role Description", "Contact Code", "Time Needed", "Time Updated", "Volunteers Confirmed"
			});
			var sites = DodoServer.SiteManager.GetSites();
			foreach (var needKey in Data.CurrentNeeds.OrderBy(x => x.Value.TimeOfRequest))
			{
				var need = needKey.Value;
				string siteName = "OFFSITE";
				string siteCode = "";
				var site = sites.FirstOrDefault(x => x.SiteCode == need.SiteCode);
				if(site != null)
				{
					siteName = site.SiteName;
					siteCode = site.SiteCode.ToString();
				}
				needs.Add(new List<string>()
				{
					siteName, siteCode, need.WorkingGroup.ParentGroup.ToString(), need.WorkingGroup.Name,
					(need.Amount == int.MaxValue  ? "MANY" : need.Amount.ToString()), need.Description, need.Key,
					(need.TimeNeeded == DateTime.MaxValue ? "NOW" : need.TimeNeeded.ToString()), need.TimeOfRequest.ToString(), need.ConfirmedVolunteers.Count.ToString(),
				});
			}
			if(!DodoServer.Dummy)
			{
				GSheets.ClearSheet(m_dataOutputSpreadsheetID, "Needs!A1:ZZZ");
				GSheets.WriteSheet(m_dataOutputSpreadsheetID, needs, "Needs!A1:ZZZ");
			}


			var checkins = new List<List<string>>();
			checkins.Add(new List<string>()
			{
				"Site", "SiteCode", "Parent Group", "Working Group", "Working Group Code", "Name", "Phone", "Current Volunteer Estimate", "Stress Level", "Understaffed Level", "Unsafe Level", "Time Updated"
			});
			foreach (var checkin in Data.Checkins.OrderBy(x => x.Value.TimeSubmitted))
			{
				var ck = checkin.Value;
				if(!DodoServer.SiteManager.IsValidWorkingGroup(ck.WorkingGroupCode))
				{
					continue;
				}

				var site = DodoServer.SiteManager.GetSite(ck.SiteCode);
				var wg = DodoServer.SiteManager.GetWorkingGroup(ck.WorkingGroupCode);
				var user = DodoServer.SessionManager.GetUserFromUserID(checkin.Key);
				checkins.Add(new List<string>()
				{
					site.SiteName, ck.SiteCode.ToString(), wg.ParentGroup.ToString(), wg.Name, wg.ShortCode, user.Name, user.PhoneNumber,
					ck.VolunteerCount.ToString(), ck.Stressed.ToString(), ck.Understaffed.ToString(), ck.Unsafe.ToString(), ck.TimeSubmitted.ToString()
				});
			}
			if (!DodoServer.Dummy)
			{
				GSheets.ClearSheet(m_dataOutputSpreadsheetID, "Checkins!A1:ZZZ");
				GSheets.WriteSheet(m_dataOutputSpreadsheetID, checkins, "Checkins!A1:ZZZ");
			}

			var population = new List<List<string>>();
			population.Add(new List<string>()
			{
				"Site", "SiteCode", "Count"
			});
			foreach (var site in DodoServer.SiteManager.GetSites())
			{
				population.Add(new List<string>()
				{
					site.SiteName, site.SiteCode.ToString(), DodoServer.SessionManager.GetUsers().Where(user => user.SiteCode == site.SiteCode).Count().ToString(),
				});
			}
			if (!DodoServer.Dummy)
			{
				GSheets.ClearSheet(m_dataOutputSpreadsheetID, "Population!A1:ZZZ");
				GSheets.WriteSheet(m_dataOutputSpreadsheetID, population, "Population!A1:ZZZ");
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
