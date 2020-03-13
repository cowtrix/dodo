using Common.Security;
using Dodo.Users;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dodo.Users.Tokens;
using Resources;

namespace Dodo.Security
{
	public class UserTokenWorker
	{
		public UserTokenWorker()
		{
			var usermanager = ResourceUtility.GetManager<User>();
			Task expireTokenTask = new Task(async () =>
			{
				var expiredTokens = new List<Guid>();
				while (true)
				{
					foreach (var user in usermanager.Get(r => true))
					{
						ExpireTokensForUser(user);
					}
				}
			});
			expireTokenTask.Start();
		}

		private static void ExpireTokensForUser(User user)
		{
			var expiredTokens = new List<Guid>();
			foreach (var token in user.TokenCollection.Tokens.OfType<IExpiringToken>())
			{
				if (token.IsExpired)
				{
					expiredTokens.Add(token.GUID);
				}
			}
			if (!expiredTokens.Any())
			{
				return;
			}
			using (var rscLock = new ResourceLock(user))
			{
				var newUser = rscLock.Value as User;
				foreach (var guid in expiredTokens)
				{
					newUser.TokenCollection.Remove(newUser, guid);
				}
			}
		}
	}
}
