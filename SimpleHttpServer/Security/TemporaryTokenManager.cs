using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Security
{
	/// <summary>
	/// This is a collection of tokens that are used to create temporary
	/// expiring encrypted stores. Every minute a new token is added and
	/// the most recent removed, and there are always 5 tokens.
	/// That means if you encrypt an object with the newest token, it
	/// will last for 5 minutes. The primary use of this is with the
	/// Passphrase struct, to create temporary objects that can produce
	/// the passphrase, without passing it around into methods in plaintext
	/// </summary>
	internal static class TemporaryTokenManager
	{
		private static ConcurrentQueue<string> m_tokens = new ConcurrentQueue<string>();
		const byte TOKEN_COUNT = 5;
		const byte TOKEN_TIMEOUT_MINUTES = 1;
		const byte TOKEN_SIZE = 32;

		static TemporaryTokenManager()
		{
			while (m_tokens.Count <= TOKEN_COUNT)
			{
				m_tokens.Enqueue(KeyGenerator.GetUniqueKey(TOKEN_SIZE));
			}
			Task tokenTask = new Task(async () =>
			{
				while (true)
				{
					await Task.Delay(TimeSpan.FromMinutes(TOKEN_TIMEOUT_MINUTES));
					m_tokens.TryDequeue(out _);
					m_tokens.Enqueue(KeyGenerator.GetUniqueKey(TOKEN_SIZE));
				}
			});
			tokenTask.Start();
		}

		public static string GetTemporaryToken()
		{
			return m_tokens.Last();
		}

		public static bool IsValidToken(string token)
		{
			return m_tokens.Contains(token);
		}
	}
}