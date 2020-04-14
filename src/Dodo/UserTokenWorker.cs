using Dodo.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dodo.Users.Tokens;
using Resources;
using Common;
using Common.Config;

namespace Dodo.Security
{
	public class UserTokenWorker
	{
		private IResourceManager<User> UserManager => ResourceUtility.GetManager<User>();
		private int TokenWorkerWaitSeconds => ConfigManager.GetValue($"{nameof(UserTokenWorker)}_{nameof(TokenWorkerWaitSeconds)}", 10);

		public UserTokenWorker()
		{
			Task expireTokenTask = new Task(async () =>
			{
				int timeout = TokenWorkerWaitSeconds;
				Logger.Debug($"Starting token worker loop with timeout {timeout}s");
				var expiredTokens = new List<Guid>();
				while (true)
				{
					foreach (var user in UserManager.Get(r => true))
					{
						RemoveTokensForUser(user);
					}
					await Task.Delay(TimeSpan.FromSeconds(timeout));
				}
			});
			expireTokenTask.Start();
		}

		private void RemoveTokensForUser(User user)
		{
			var removedTokens = new List<Guid>();
			foreach (var token in user.TokenCollection.Tokens.OfType<IRemovableToken>())
			{
				if (token.ShouldRemove)
				{
					removedTokens.Add(token.Guid);
				}
			}
			if (!removedTokens.Any())
			{
				return;
			}
			Logger.Debug($"Removing {removedTokens.Count} tokens from user {user}");
			using (var rscLock = new ResourceLock(user))
			{
				var newUser = rscLock.Value as User;
				foreach (var guid in removedTokens)
				{
					if(!newUser.TokenCollection.Remove(newUser, guid))
					{
						Logger.Warning($"Unexpectedly failed to removed token from user {user.Guid} with token Guid {guid}");
					}
				}
				UserManager.Update(newUser, rscLock);
			}
		}
	}
}
