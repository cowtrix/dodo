using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;

namespace XR.Dodo
{
	public class SessionManager
	{
		JsonSerializer m_serializer = new JsonSerializer();

		public SessionManager(string dataPath)
		{
			if(!File.Exists(dataPath))
			{
				File.Create(dataPath).Close();
			}
			using (var jsonReader = new JsonTextReader(File.OpenText(dataPath)))
			{
				_sessions = m_serializer.Deserialize<Dictionary<User, UserSession>>(jsonReader) ??
					new Dictionary<User, UserSession>();
			}
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

		private Dictionary<User, UserSession> _sessions;

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
