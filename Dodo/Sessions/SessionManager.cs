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
			if(DodoServer.Dummy)
			{
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
			Logger.Debug($"Saved user session data to {dataPath}");
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

		public void TryVerify(string fromNumber, string code)
		{
			if (!string.IsNullOrEmpty(code))
			{
				var verificationMatch = _data.Sessions
					.FirstOrDefault(x => (x.Value.Verification?.Code == code.Trim()));
				if (verificationMatch.Value == null)
				{
					Logger.Warning($"User {fromNumber} sent invalid code to validation number: {code}");
					return;
				}
				if (!ValidationExtensions.ValidateNumber(ref fromNumber))
				{
					throw new Exception("Invalid number: " + fromNumber);
				}
				var userToVerify = verificationMatch.Value.GetUser();
				var existingUserWithNumber = _data.Users.FirstOrDefault(x => x.Value.PhoneNumber == fromNumber).Value;
				string toRemove = existingUserWithNumber?.UUID;
				if(existingUserWithNumber != null)
				{
					// Someone is already registered in the database with that phone number
					// Which should only really ever happen with a coordinator
					// So we copy over some stuff and then delete the existing user
					RemoveUser(existingUserWithNumber);
					userToVerify.Email = existingUserWithNumber.Email ?? userToVerify.Email;
					foreach (var role in existingUserWithNumber.CoordinatorRoles)
					{
						userToVerify.CoordinatorRoles.Add(role);
					}
				}
				
				userToVerify.PhoneNumber = fromNumber;
				userToVerify.Karma += 10;
				if (userToVerify.AccessLevel > EUserAccessLevel.Volunteer)
				{
					DodoServer.TelegramGateway.SendMessage(new ServerMessage($"Awesome! You've verified your number as {userToVerify.PhoneNumber}. " + 
						$"It looks like you're a coordinator for {userToVerify.CoordinatorRoles.First().WorkingGroup.Name}"), verificationMatch.Value);
				}
				else
				{
					DodoServer.TelegramGateway.SendMessage(new ServerMessage($"Awesome! You've verified your number as {userToVerify.PhoneNumber}."), verificationMatch.Value);
				}
				Logger.Debug($"Succesfully verified user {userToVerify} to number {userToVerify.PhoneNumber}");
			}
		}

		private bool RemoveUser(User user)
		{
			Logger.Debug("Removed user " + user.UUID);
			return _data.Users.TryRemove(user.UUID, out _) &&
			_data.Sessions.TryRemove(user.UUID, out _);
		}

		public UserSession GetSessionFromUserID(string ownerUID)
		{
			return _data.Sessions.FirstOrDefault(x => x.Key == ownerUID).Value;
		}

		public User GetUserFromUserID(string ownerUID)
		{
			if(!_data.Users.TryGetValue(ownerUID, out var result))
			{
				throw new Exception("Could not find user with id " + ownerUID);
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
			var user = _data.Users.FirstOrDefault(x => x.Value.PhoneNumber == fromNumber).Value;
			if(user == null)
			{
				user = new User() { PhoneNumber = fromNumber };
				_data.Users.TryAdd(user.UUID, user);
			}
			return user;
		}

		public User GetOrCreateUserFromTelegramNumber(int telegramId)
		{
			var user = _data.Users.FirstOrDefault(x => x.Value.TelegramUser == telegramId).Value;
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
