using Common.Security;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resources.Security
{
	/// <summary>
	/// This is a collection of tokens that are used to create temporary
	/// expiring encrypted stores.
	/// </summary>
	public static class SessionTokenManager
	{
		public class SessionToken
		{
			public string UserToken;
			public string Value;
			public TimeSpan Timeout;
			[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
			public DateTime CreationDate;

			public bool IsExpired => CreationDate + Timeout < DateTime.Now;
		}

		private static PersistentStore<string, SessionToken> m_tokens = new PersistentStore<string, SessionToken>("auth", "temptokens");
		const byte DEFAULT_TOKEN_TIMEOUT_MINUTES = 5;
		const byte TOKEN_SIZE = 32;

		static SessionTokenManager()
		{
			Task tokenTask = new Task(async () =>
			{
				while (true)
				{
					await Task.Delay(TimeSpan.FromMinutes(1));
					foreach(var token in m_tokens.GetQueryable())
					{
						if(token.Value.IsExpired)
						{
							m_tokens.Remove(token.Key);
						}
					}
				}
			});
			tokenTask.Start();
		}

		public static void GetTemporaryToken(out string token, out string key, TimeSpan? timeout = null)
		{
			token = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
			SetTemporaryToken(token, out key, timeout);
		}

		public static void SetTemporaryToken(string token, out string key, TimeSpan? timeout = null)
		{
			timeout = timeout ?? TimeSpan.FromMinutes(DEFAULT_TOKEN_TIMEOUT_MINUTES);
			key = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
			m_tokens[key] = new SessionToken()
			{
				CreationDate = DateTime.Now,
				Value = token,
				Timeout = timeout.Value,
			};
		}

		public static bool CheckToken(string key, out string token)
		{
			if(!m_tokens.TryGetValue(key, out var innerToken))
			{
				token = null;
				return false;
			}
			token = innerToken.Value;
			return true;
		}
	}
}
