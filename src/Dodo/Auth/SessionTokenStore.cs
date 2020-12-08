using Common;
using Common.Security;
using Dodo.Users;
using Dodo.Users.Tokens;
using MongoDB.Bson.Serialization.Attributes;
using Resources;
using Resources.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dodo.Security
{
	public static class SessionTokenStore
	{
		public class SessionKey
		{
			[BsonElement]
			public DateTime DateCreated { get; set; }
			[BsonElement]
			public SymmEncryptedStore<Guid> EncryptedGUID { get; set; }
		}

		static IResourceManager<User> UserManager = ResourceUtility.GetManager<User>();
		static PersistentStore<string, SessionKey> m_tokenStore = new PersistentStore<string, SessionKey>("auth", "session");

		public static void Initialise()
		{
			var trimTask = new Task(async () =>
			{
				var toRemove = new List<string>();
				while (true)
				{
					toRemove.Clear();
					var now = DateTime.Now;
					foreach (var token in m_tokenStore.GetQueryable())
					{
						if(now > token.Value.DateCreated + SessionToken.LongSessionExpiryTime)
						{
							toRemove.Add(token.Key);
						}
					}
					if (toRemove.Any())
					{
						foreach(var key in toRemove)
						{
							if (!m_tokenStore.Remove(key))
							{
								Logger.Error($"Unexpectededly failed to remove expired session token {key}");
							}
						}
						Logger.Debug($"SessionTokenStore removed {toRemove.Count} expired session tokens");
					}
					await Task.Delay(TimeSpan.FromMinutes(10));
				}
			});
			trimTask.Start();
		}

		public static User GetUser(string userToken, string sessionKey)
		{
			if(!m_tokenStore.TryGetValue(userToken, out var userEncryptedGuid))
			{
				return null;
			}
			var userGuid = userEncryptedGuid.EncryptedGUID.GetValue(sessionKey);
			return UserManager.GetSingle(u => u.Guid == userGuid);
		}

		public static void SetUser(string userToken, Passphrase sessionKey, Guid userGuid)
		{
			m_tokenStore[userToken] = new SessionKey
			{ 
				EncryptedGUID = new SymmEncryptedStore<Guid>(userGuid, sessionKey),
				DateCreated = DateTime.Now
			};
		}

		public static void RemoveUser(string userToken)
		{
			if(!m_tokenStore.Remove(userToken))
			{
				Logger.Warning($"Unexpectedly failed to remove session token {userToken}");
			}
		}
	}
}
