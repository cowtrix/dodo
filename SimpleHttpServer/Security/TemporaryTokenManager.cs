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
		private static ConcurrentDictionary<string, DateTime> m_tokens = new ConcurrentDictionary<string, DateTime>();
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
					var toRemove = m_tokens.Keys.Where(key => m_tokens[key] < cutoff).ToList();
					foreach(var val in toRemove)
					{
						m_tokens.TryRemove(val, out _);
					}
				}
			});
			tokenTask.Start();
		}

		public static string GetTemporaryToken()
		{
			var token = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
			m_tokens[SHA256Utility.SHA256(token)] = DateTime.Now;
			return token;
		}

		public static bool IsValidToken(string tokenKey)
		{
			tokenKey = SHA256Utility.SHA256(tokenKey);
			return m_tokens.ContainsKey(tokenKey);
		}
	}
}