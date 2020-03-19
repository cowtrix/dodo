using Common.Security;
using Dodo.Users;
using Resources;
using Resources.Security;
using System;

namespace Dodo.Security
{
	public static class SessionTokenStore
	{
		public class SessionKey
		{
			public DateTime DateCreated;
			public EncryptedStore<Guid> EncryptedGUID;
		}

		static IResourceManager<User> UserManager = ResourceUtility.GetManager<User>();
		static PersistentStore<string, SessionKey> m_tokenStore = new PersistentStore<string, SessionKey>("auth", "session");

		public static User GetUser(string userToken, string sessionKey)
		{
			if(!m_tokenStore.TryGetValue(userToken, out var userEncryptedGuid))
			{
				return null;
			}
			var userGuid = userEncryptedGuid.EncryptedGUID.GetValue(sessionKey);
			return UserManager.GetSingle(u => u.GUID == userGuid);
		}

		public static void SetUser(string userToken, Passphrase sessionKey, Guid userGuid)
		{
			m_tokenStore[userToken] = new SessionKey
			{ 
				EncryptedGUID = new EncryptedStore<Guid>(userGuid, sessionKey),
				DateCreated = DateTime.Now
			};
		}
	}
}
