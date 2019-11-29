using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Security
{
	/// <summary>
	/// This is a collection of tokens that are used to create temporary
	/// expiring encrypted stores.
	/// </summary>
	internal static class TemporaryTokenManager
	{
		private struct InnerToken
		{
			public string Value;
			public DateTime CreationDate;
		}

		private static ConcurrentDictionary<string, InnerToken> m_tokens = new ConcurrentDictionary<string, InnerToken>();
		const byte TOKEN_TIMEOUT_MINUTES = 5;
		const byte TOKEN_SIZE = 32;

		static TemporaryTokenManager()
		{
			Task tokenTask = new Task(async () =>
			{
				while (true)
				{
					await Task.Delay(TimeSpan.FromMinutes(1));
					var cutoff = DateTime.Now - TimeSpan.FromMinutes(TOKEN_TIMEOUT_MINUTES);
					var toRemove = m_tokens.Keys.Where(key => m_tokens[key].CreationDate < cutoff).ToList();
					foreach(var val in toRemove)
					{
						m_tokens.TryRemove(val, out _);
					}
				}
			});
			tokenTask.Start();
		}

		public static void GetTemporaryToken(out string tokenKey, out string token)
		{
			tokenKey = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
			token = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
			m_tokens[SHA256Utility.SHA256(tokenKey)] = new InnerToken()
			{
				CreationDate = DateTime.Now,
				Value = token,
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