using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace XR.Dodo
{
	public class SessionManager
	{
		readonly string m_dataPath;
		public class SessionManagerData
		{
			public ConcurrentDictionary<string, User> Users = new ConcurrentDictionary<string, User>();
			public ConcurrentDictionary<string, UserSession> Sessions = new ConcurrentDictionary<string, UserSession>();
		}

		private SessionManagerData _data = new SessionManagerData();

		public SessionManager(string dataPath)
		{
			m_dataPath = dataPath;
			LoadSessions();
			var updateTask = new Task(() =>
			{
				while(true)
				{
					Thread.Sleep(10 * 1000);
					SaveSessions();
				}
			});
			updateTask.Start();
		}

		void LoadSessions()
		{
			if (!File.Exists(m_dataPath))
			{
				File.Create(m_dataPath).Close();
			}
			try
			{
				//var decrypted = EncryptStringSample.StringCipher.Decrypt(File.ReadAllText(m_dataPath), DodoServer.SessionPassword);
				_data = JsonConvert.DeserializeObject<SessionManagerData>(File.ReadAllText(m_dataPath), new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				}) ?? _data;
			}
			catch(Exception e)
			{
				Logger.Debug($"Failed to deserialize sessions: {e.Message}\n{e.StackTrace}");
			}
		}

		void SaveSessions()
		{
			if (!File.Exists(m_dataPath))
			{
				File.Create(m_dataPath).Close();
			}
			File.WriteAllText(m_dataPath, JsonConvert.SerializeObject(_data, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
			Logger.Debug($"Saved user session data to {m_dataPath}");
		}

		public User MergeUsers(User first, User second)
		{
			if((!string.IsNullOrEmpty(first.PhoneNumber) || !string.IsNullOrEmpty(second.PhoneNumber))
				&& first.PhoneNumber != second.PhoneNumber)
			{
				throw new Exception("Couldn't merge. Numbers conflicted!");
			}
			if ((first.TelegramUser != 0 || second.TelegramUser != 0)
				&& first.TelegramUser != second.TelegramUser)
			{
				throw new Exception("Couldn't merge. Numbers conflicted!");
			}
			if (first.Name != second.Name)
			{
				first.Name += "/" + second.Name;
			}
			first.PhoneNumber = !string.IsNullOrEmpty(first.PhoneNumber) ? first.PhoneNumber : second.PhoneNumber;
			first.TelegramUser = first.TelegramUser != 0 ? first.TelegramUser : second.TelegramUser;
			return first;
		}

		public UserSession GetOrCreateSession(User user)
		{
			if(!_data.Users.TryGetValue(user.UUID, out var existingUser))
			{
				Logger.Debug("Added missing user when getting session");
				_data.Users.TryAdd(user.UUID, user);
			}
			else if(!ReferenceEquals(user, existingUser))
			{
				throw new Exception($"Duplicate GUID found - a user object probably got copied somewhere.");
			}
			if (!_data.Sessions.TryGetValue(user.UUID, out var session))
			{
				if(user.AccessLevel > EUserAccessLevel.Volunteer)
				{
					session = new CoordinatorSession(user.UUID);
				}
				else
				{
					session = new VolunteerSession(user.UUID);
				}
				if(!_data.Sessions.TryAdd(user.UUID, session))
				{
					return _data.Sessions[user.UUID];
				}
			}
			return session;
		}

		public UserSession GetSessionFromUserID(string ownerUID)
		{
			return _data.Sessions.FirstOrDefault(x => x.Key == ownerUID).Value;
		}

		public User GetUserFromUserID(string ownerUID)
		{
			_data.Users.TryGetValue(ownerUID, out var result);
			return result;
		}

		public User GetOrCreateUserFromPhoneNumber(string fromNumber, string messageString = null)  
		{
			if(!ValidationExtensions.ValidateNumber(ref fromNumber))
			{
				return null;
			}
			if (!string.IsNullOrEmpty(messageString))
			{
				// Reroute SMS validation
				var verificationMatch = _data.Sessions
					.FirstOrDefault(x => (x.Value.Workflow.CurrentTask as Verification)?
					.CodeString == messageString);
				if (verificationMatch.Value != null)
				{
					return verificationMatch.Value.GetUser();
				}
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
