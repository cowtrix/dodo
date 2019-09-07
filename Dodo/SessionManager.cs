using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace XR.Dodo
{
	public class SessionManager
	{
		readonly string m_dataPath;
		private Dictionary<User, UserSession> _sessions;

		public SessionManager(string dataPath)
		{
			_sessions = new Dictionary<User, UserSession>();
			m_dataPath = dataPath;
			LoadSessions();
			var updateTask = new Task(() =>
			{
				while(true)
				{
					Thread.Sleep(10 * 10000);
					SaveSessions();
				}
			});
			updateTask.Start();
			SaveSessions();
		}

		void LoadSessions()
		{
			if (!File.Exists(m_dataPath))
			{
				File.Create(m_dataPath).Close();
			}
			var sessions = JsonConvert.DeserializeObject<List<UserSession>>(File.ReadAllText(m_dataPath)) ??
					new List<UserSession>();
			_sessions.Clear();
			foreach (var session in sessions)
			{
				_sessions.Add(session.User, session);
			}
		}

		public UserSession GetSessionFromUUID(string ownerUID)
		{
			return _sessions.FirstOrDefault(x => x.Key.UUID == ownerUID).Value;
		}

		void SaveSessions()
		{
			if (!File.Exists(m_dataPath))
			{
				File.Create(m_dataPath).Close();
			}
			File.WriteAllText(m_dataPath, JsonConvert.SerializeObject(_sessions.Values.ToList(), Formatting.Indented));
			Console.WriteLine($"Saved user session data to {m_dataPath}");
		}

		public UserSession GetOrCreateSession(User user)
		{
			if (!_sessions.TryGetValue(user, out var session))
			{
				session = new UserSession(user);
				_sessions.Add(user, session);
			}
			return session;
		}

		public UserSession GetOrCreateSessionFromNumber(string fromNumber)
		{
			if(!PhoneExtensions.ValidateNumber(ref fromNumber))
			{
				return null;
			}
			var match = _sessions.FirstOrDefault(x => x.Value.User.PhoneNumber == fromNumber).Value;
			if(match == null)
			{
				var user = new User()
				{
					PhoneNumber = fromNumber,
				};
				match = new UserSession(user);
				_sessions[user] = match;
			}
			return match;
		}

		public UserSession GetOrCreateSessionFromTelegramName(int telegramId)
		{
			var match = _sessions.FirstOrDefault(x => x.Value.User.TelegramUser == telegramId).Value;
			if (match == null)
			{
				var user = new User()
				{
					TelegramUser = telegramId,
				};
				match = new UserSession(user);
				_sessions[user] = match;
			}
			return match;
		}
	}
}
