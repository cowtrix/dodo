using Common.Security;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Security
{
	/// <summary>
	/// This is a collection of tokens that are used to create temporary
	/// expiring encrypted stores.
	/// </summary>
	public static class TemporaryTokenManager
	{
		private struct InnerToken
		{
			public string Value;
			public TimeSpan Timeout;
			public DateTime CreationDate;
		}

		private static ConcurrentDictionary<string, InnerToken> m_tokens = new ConcurrentDictionary<string, InnerToken>();
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
					var now = DateTime.Now;
					foreach (var val in m_tokens)
					{
						if(val.Value.CreationDate > now - val.Value.Timeout)
						{
							continue;
						}
						toRemove.Add(val.Key);
					}
					foreach(var r in toRemove)
					{
						m_tokens.TryRemove(r, out _);
					}
				}
			});
			tokenTask.Start();
		}

		public static void GetTemporaryToken(out string tokenKey, out string token, TimeSpan? timeout = null)
		{
			timeout = timeout ?? TimeSpan.FromMinutes(DEFAULT_TOKEN_TIMEOUT_MINUTES);
			tokenKey = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
			token = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
			m_tokens[SHA256Utility.SHA256(tokenKey)] = new InnerToken()
			{
				CreationDate = DateTime.Now,
				Value = token,
				Timeout = timeout.Value,
			};
		}

		public static bool IsValidToken(string tokenKey, out string token)
		{
			tokenKey = SHA256Utility.SHA256(tokenKey);
			if(!m_tokens.TryGetValue(tokenKey, out var innerToken))
			{
				token = null;
				return false;
			}
			token = innerToken.Value;
			return true;
		}
	}
}