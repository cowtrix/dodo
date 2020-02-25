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
	public static class TemporaryTokenManager
	{
		private class InnerToken
		{
			[BsonElement]
			public string Value;
			[BsonElement]
			public long Timeout;
			[BsonElement]
			public long CreationDate;
		}

		private static PersistentStore<string, InnerToken> m_tokens = new PersistentStore<string, InnerToken>("auth", "temptokens");
		const byte DEFAULT_TOKEN_TIMEOUT_MINUTES = 5;
		const byte TOKEN_SIZE = 32;

		static TemporaryTokenManager()
		{
			Task tokenTask = new Task(async () =>
			{
				var toRemove = new List<string>();
				while (true)
				{
					await Task.Delay(TimeSpan.FromMinutes(1));
					var now = DateTime.Now.Ticks;
					/*var outdatedTokens = m_tokens.GetQueryable().Where(val => val.Value.CreationDate < now - val.Value.Timeout).ToList();
					foreach(var r in outdatedTokens)
					{
						// Expire tokens
						m_tokens.Remove(r.Key);
					}*/
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
			m_tokens[key] = new InnerToken()
			{
				CreationDate = DateTime.Now.Ticks,
				Value = token,
				Timeout = timeout.Value.Ticks,
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
