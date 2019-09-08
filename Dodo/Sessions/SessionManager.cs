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
			public ConcurrentBag<User> Users = new ConcurrentBag<User>();
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
				_data = JsonConvert.DeserializeObject<SessionManagerData>(File.ReadAllText(m_dataPath), new JsonSerializerSettings
				{
					TypeNameHandling = TypeNameHandling.Auto
				}) ?? _data;
			}
			catch(Exception e)
			{
				Console.WriteLine($"Failed to deserialize sessions: {e.Message}\n{e.StackTrace}");
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
			Console.WriteLine($"Saved user session data to {m_dataPath}");
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
			if (!_data.Sessions.TryGetValue(user.UUID, out var session))
			{
				if(user is Volunteer)
				{
					session = new VolunteerSession(user as Volunteer);
				}
				else if(user is Coordinator)
				{
					session = new CoordinatorSession(user as Coordinator);
				}
				else
				{
					return null;
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
			return _data.Users.FirstOrDefault(x => x.UUID == ownerUID);
		}

		public User GetOrCreateUserFromPhoneNumber(string fromNumber)  
		{
			if(!PhoneExtensions.ValidateNumber(ref fromNumber))
			{
				return null;
			}
			var user = _data.Users.FirstOrDefault(x => x.PhoneNumber == fromNumber);
			if(user == null)
			{
				user = new Volunteer() { PhoneNumber = fromNumber };
				_data.Users.Add(user);
			}
			return user;
		}

		public User GetOrCreateUserFromTelegramNumber(int telegramId)
		{
			var user = _data.Users.FirstOrDefault(x => x.TelegramUser == telegramId);
			if (user == null)
			{
				user = new Volunteer() { TelegramUser = telegramId };
				_data.Users.Add(user);
			}
			return user;
		}

		public List<UserSession> GetCurrentSessions()
		{
			return _data.Sessions.Values.ToList();
		}

		public void AddUser(Coordinator coordinator)
		{
			_data.Users.Add(coordinator);
		}
	}
}
