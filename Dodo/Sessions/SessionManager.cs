using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using Common;

namespace XR.Dodo
{
	public class SessionManager : IBackup
	{
		public class SessionManagerData
		{
			public ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>();
			public ConcurrentDictionary<string, UserSession> Sessions = new ConcurrentDictionary<string, UserSession>();
		}

		private SessionManagerData _data = new SessionManagerData();

		public SessionManager(Configuration config)
		{
			LoadFromFile(config.BackupPath);
		}

		public void LoadFromFile(string backupFolder)
		{
			if(DodoServer.NoLoad || DodoServer.Dummy)
			{
				_data = _data ?? new SessionManagerData();
				return;
			}
			var dataPath = Path.Combine(backupFolder, "sessions.json");
			if (!File.Exists(dataPath))
			{
				File.Create(dataPath).Close();
			}
			try
			{
				_data = JsonConvert.DeserializeObject<SessionManagerData>(File.ReadAllText(dataPath), new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				}) ?? _data;
				Logger.Debug($"Loaded user session data from {dataPath}");
			}
			catch(Exception e)
			{
				Logger.Debug($"Failed to deserialize sessions: {e.Message}\n{e.StackTrace}");
			}
		}

		public void SaveToFile(string backupFolder)
		{
			if (DodoServer.Dummy)
			{
				return;
			}
			var dataPath = Path.Combine(backupFolder, "sessions.json");
			File.WriteAllText(dataPath, JsonConvert.SerializeObject(_data, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
			//Logger.Debug($"Saved user session data to {dataPath}");
		}

		public List<User> GetUsers()
		{
			return _data.Users.Values.ToList();
		}

		public UserSession GetOrCreateSession(User user)
		{
			if(!_data.Users.TryGetValue(user.UUID, out var existingUser))
			{
				if(DodoServer.Dummy)
				{
					Logger.Debug("Added missing user when getting session");
					_data.Users.TryAdd(user.UUID, user);
				}
				else
				{
					throw new Exception("Not allowed out of testing mode");
				}
			}
			else if(!ReferenceEquals(user, existingUser))
			{
				throw new Exception($"Duplicate GUID found - a user object probably got copied somewhere.");
			}
			if (!_data.Sessions.TryGetValue(user.UUID, out var session))
			{
				session = new UserSession(user.UUID);
				if (!_data.Sessions.TryAdd(user.UUID, session))
				{
					return _data.Sessions[user.UUID];
				}
			}
			return session;
		}

		public void TryVerify(string phoneNumber, int telegramUser)
		{
			var originalNumber = phoneNumber;
			if (!ValidationExtensions.ValidateNumber(ref phoneNumber))
			{
				DodoServer.TelegramGateway.SendMessage(new ServerMessage($"Your number - {originalNumber} - doesn't seem like a valid mobile phone number. If you think this is a mistake, please contact the Rota Team Rebellion Bot support."), telegramUser);
				Logger.Error("Invalid number: " + phoneNumber);
				return;
			}
			var existingTelegramUser = GetOrCreateUserFromTelegramNumber(telegramUser);
			var session = GetOrCreateSession(existingTelegramUser);
			if (existingTelegramUser.IsVerified())
			{
				DodoServer.TelegramGateway.SendMessage(new ServerMessage($"You've already verified your number with me. Thanks though!"), session);
				return;
			}
			var existingPhoneUser = _data.Users.FirstOrDefault(x => x.Value.PhoneNumber == phoneNumber).Value;
			string toRemove = existingPhoneUser?.UUID;
			if(existingPhoneUser != null)
			{
				// Someone is already registered in the database with that phone number
				// Which should only really ever happen with a coordinator
				// So we copy over some stuff and then delete the existing user
				RemoveUser(existingPhoneUser);
				existingTelegramUser.Name = existingTelegramUser.Name ?? existingPhoneUser.Name;
				existingTelegramUser.Email = existingPhoneUser.Email ?? existingPhoneUser.Email;
				foreach (var role in existingPhoneUser.CoordinatorRoles)
				{
					existingTelegramUser.CoordinatorRoles.Add(role);
				}
			}

			existingTelegramUser.PhoneNumber = phoneNumber;
			existingTelegramUser.Karma += 10;
			if (existingTelegramUser.AccessLevel > EUserAccessLevel.Volunteer)
			{
				DodoServer.DefaultGateway.SendMessage(new ServerMessage($"Hi {existingTelegramUser.Name}! You've verified your number as {existingTelegramUser.PhoneNumber}. " +
					$"It looks like you're a coordinator for {existingTelegramUser.CoordinatorRoles.First().WorkingGroup.Name}"), session);
			}
			else
			{
				DodoServer.DefaultGateway.SendMessage(new ServerMessage($"Awesome! You've verified your number as {existingTelegramUser.PhoneNumber}."), session);
			}
			Logger.Debug($"Succesfully verified user {existingTelegramUser.TelegramUser} to number {existingTelegramUser.PhoneNumber}");

			if(session.Workflow.CurrentTask is IntroductionTask)
			{
				DodoServer.TelegramGateway.SendMessage(session.ProcessMessage(new UserMessage(existingTelegramUser,
					"", DodoServer.TelegramGateway, existingTelegramUser.TelegramUser.ToString()), session), session);
			}
		}

		public bool RemoveUser(User user)
		{
			Logger.Debug("Removed user " + user.UUID);
			DodoServer.CoordinatorNeedsManager.Data.Checkins.TryRemove(user.UUID, out _);
			DodoServer.CoordinatorNeedsManager.Data.CheckinReminders.TryRemove(user.UUID, out _);

			return _data.Users.TryRemove(user.UUID, out _) &&
			_data.Sessions.TryRemove(user.UUID, out _);
		}

		public UserSession GetSessionFromUserID(string ownerUID)
		{
			return _data.Sessions.SingleOrDefault(x => x.Key == ownerUID).Value;
		}

		public User GetUserFromUserID(string ownerUID)
		{
			if(!_data.Users.TryGetValue(ownerUID, out var result))
			{
				return null;
			}
			return result;
		}

		public User GetOrCreateUserFromPhoneNumber(string fromNumber)
		{
			if(string.IsNullOrEmpty(fromNumber))
			{
				return null;
			}
			if(!ValidationExtensions.ValidateNumber(ref fromNumber))
			{
				return null;
			}
			var user = _data.Users.Where(x => x.Value.PhoneNumber == fromNumber)
				.OrderByDescending(i => i.Value.AccessLevel).FirstOrDefault().Value;
			if(user == null)
			{
				user = new User() { PhoneNumber = fromNumber };
				_data.Users.TryAdd(user.UUID, user);
			}
			return user;
		}

		public User GetOrCreateUserFromTelegramNumber(int telegramId)
		{
			var user = _data.Users.SingleOrDefault(x => x.Value.TelegramUser == telegramId).Value;
			if (user == null)
			{
				user = new User() { TelegramUser = telegramId };
				_data.Users.TryAdd(user.UUID, user);
			}
			return user;
		}

		public List<UserSession> GetCurrentSessions()
		{
			return _data.Sessions.Values.ToList();
		}
	}
}
