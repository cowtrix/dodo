
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
// I'm using async methods to leverage implicit Task wrapping of results from expression bodied functions.

namespace Microsoft.AspNetCore.Identity.MongoDB
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
	using System.Threading;
	using System.Threading.Tasks;
    using Dodo.Users;
    using global::MongoDB.Bson;
	using global::MongoDB.Driver;
    using REST;
	using Common.Extensions;

    /// <summary>
    ///     When passing a cancellation token, it will only be used if the operation requires a database interaction.
    /// </summary>
    public class UserStore :
			IUserPasswordStore<User>,
			IUserRoleStore<User>,
			IUserLoginStore<User>,
			IUserSecurityStampStore<User>,
			IUserEmailStore<User>,
			IUserClaimStore<User>,
			IUserPhoneNumberStore<User>,
			IUserTwoFactorStore<User>,
			IUserLockoutStore<User>,
			IQueryableUserStore<User>,
			IUserAuthenticationTokenStore<User>
	{
		private readonly DodoUserManager UserManager;

		public UserStore()
		{
			UserManager = ResourceUtility.GetManager<User>() as DodoUserManager;
		}

		public virtual async Task<IdentityResult> CreateAsync(User user, CancellationToken token)
		{
			UserManager.Add(user);
			return IdentityResult.Success;
		}

		public virtual async Task<IdentityResult> UpdateAsync(User user, CancellationToken token)
		{
			if(user == null)
			{
				throw new NullReferenceException("Null user");
			}
			using (var rscLock = new ResourceLock(user.GUID))
			{
				UserManager.Update(user, rscLock);
			}
			return IdentityResult.Success;
		}

		public virtual async Task<IdentityResult> DeleteAsync(User user, CancellationToken token)
		{
			UserManager.Delete(user);
			return IdentityResult.Success;
		}

		public virtual async Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
			=> user.GUID.ToString();

		public virtual async Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
			=> user.AuthData.Username;

		public virtual async Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
			=> user.AuthData.Username = userName;

		// note: again this isn't used by Identity framework so no way to integration test it
		public virtual async Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
			=> user.AuthData.Username;

		public virtual async Task SetNormalizedUserNameAsync(User user, string normalizedUserName, CancellationToken cancellationToken)
			=> user.AuthData.Username = normalizedUserName.StripForURL();

		public virtual async Task<User> FindByIdAsync(string userId, CancellationToken token)
		{
			if(!Guid.TryParse(userId, out var guid))
			{
				return null;
			}
			return UserManager.GetSingle(u => u.GUID == guid);
		}

		private bool IsObjectId(string id)
		{
			ObjectId temp;
			return ObjectId.TryParse(id, out temp);
		}

		public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken token)
		{
			normalizedUserName = normalizedUserName.StripForURL();
			return UserManager.GetSingle(u => u.AuthData.Username == normalizedUserName);
		}

		public virtual async Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken token)
			=> user.AuthData.PasswordHash = passwordHash;

		public virtual async Task<string> GetPasswordHashAsync(User user, CancellationToken token)
			=> user.AuthData.PasswordHash;

		public virtual async Task<bool> HasPasswordAsync(User user, CancellationToken token)
			=> !string.IsNullOrEmpty(user.AuthData.PasswordHash);

		public virtual async Task AddToRoleAsync(User user, string normalizedRoleName, CancellationToken token)
			=> user.AuthData.Roles.Add(normalizedRoleName);

		public virtual async Task RemoveFromRoleAsync(User user, string normalizedRoleName, CancellationToken token)
			=> user.AuthData.Roles.Remove(normalizedRoleName);

		// todo might have issue, I'm just storing Normalized only now, so I'm returning normalized here instead of not normalized.
		// EF provider returns not noramlized here
		// however, the rest of the API uses normalized (add/remove/isinrole) so maybe this approach is better anyways
		// note: could always map normalized to not if people complain
		public virtual async Task<IList<string>> GetRolesAsync(User user, CancellationToken token)
			=> user.AuthData.Roles;

		public virtual async Task<bool> IsInRoleAsync(User user, string normalizedRoleName, CancellationToken token)
			=> user.AuthData.Roles.Contains(normalizedRoleName);

		public virtual async Task<IList<User>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken token)
		{
			return UserManager.Get(u => u.AuthData.Roles.Contains(normalizedRoleName)).ToList() as IList<User>;
		}

		public virtual async Task AddLoginAsync(User user, UserLoginInfo login, CancellationToken token)
			=> user.AuthData.AddLogin(login);

		public virtual async Task RemoveLoginAsync(User user, string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
			=> user.AuthData.RemoveLogin(loginProvider, providerKey);

		public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(User user, CancellationToken token)
			=> user.AuthData.Logins
				.Select(l => l.ToUserLoginInfo())
				.ToList();

		public async virtual Task<User> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UserManager
				.GetSingle(u => u.AuthData.Logins.Any(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey));
		}

		public virtual async Task SetSecurityStampAsync(User user, string stamp, CancellationToken token)
			=> user.AuthData.SecurityStamp = stamp;

		public virtual async Task<string> GetSecurityStampAsync(User user, CancellationToken token)
			=> user.AuthData.SecurityStamp;

		public virtual async Task<bool> GetEmailConfirmedAsync(User user, CancellationToken token)
			=> user.PersonalData.EmailConfirmed;

		public virtual async Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken token)
			=> user.PersonalData.EmailConfirmed = confirmed;

		public virtual async Task SetEmailAsync(User user, string email, CancellationToken token)
			=> user.PersonalData.Email = email;

		public virtual async Task<string> GetEmailAsync(User user, CancellationToken token)
			=> user.PersonalData.Email;

		// note: no way to intergation test as this isn't used by Identity framework	
		public virtual async Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
			=> user.PersonalData.Email;

		public virtual async Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
			=> user.PersonalData.Email = normalizedEmail;

		public async virtual Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken token)
		{
			return UserManager.GetSingle(u => u.PersonalData.Email == normalizedEmail);
		}

		public virtual async Task<IList<Claim>> GetClaimsAsync(User user, CancellationToken token)
			=> user.AuthData.Claims.Select(c => c.ToSecurityClaim()).ToList();

		public virtual Task AddClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken token)
		{
			foreach (var claim in claims)
			{
				user.AuthData.AddClaim(claim);
			}
			return Task.FromResult(0);
		}

		public virtual Task RemoveClaimsAsync(User user, IEnumerable<Claim> claims, CancellationToken token)
		{
			foreach (var claim in claims)
			{
				user.AuthData.RemoveClaim(claim);
			}
			return Task.FromResult(0);
		}

		public virtual async Task ReplaceClaimAsync(User user, Claim claim, Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
		{
			user.AuthData.ReplaceClaim(claim, newClaim);
		}

		public virtual Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken token)
		{
			user.PersonalData.PhoneNumber = phoneNumber;
			return Task.FromResult(0);
		}

		public virtual Task<string> GetPhoneNumberAsync(User user, CancellationToken token)
		{
			return Task.FromResult(user.PersonalData.PhoneNumber);
		}

		public virtual Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken token)
		{
			return Task.FromResult(user.PersonalData.PhoneNumberConfirmed);
		}

		public virtual Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken token)
		{
			user.PersonalData.PhoneNumberConfirmed = confirmed;
			return Task.FromResult(0);
		}

		public virtual Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken token)
		{
			user.AuthData.TwoFactorEnabled = enabled;
			return Task.FromResult(0);
		}

		public virtual Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken token)
		{
			return Task.FromResult(user.AuthData.TwoFactorEnabled);
		}

		public virtual async Task<IList<User>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken = default(CancellationToken))
		{
			return UserManager.Get(u => u.AuthData.Claims.Any(c => c.Type == claim.Type && c.Value == claim.Value)) as IList<User>;
		}

		public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync(User user, CancellationToken token)
		{
			DateTimeOffset? dateTimeOffset = user.AuthData.LockoutEndDateUtc;
			return Task.FromResult(dateTimeOffset);
		}

		public virtual Task SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd, CancellationToken token)
		{
			user.AuthData.LockoutEndDateUtc = lockoutEnd?.UtcDateTime;
			return Task.FromResult(0);
		}

		public virtual Task<int> IncrementAccessFailedCountAsync(User user, CancellationToken token)
		{
			user.AuthData.AccessFailedCount++;
			return Task.FromResult(user.AuthData.AccessFailedCount);
		}

		public virtual Task ResetAccessFailedCountAsync(User user, CancellationToken token)
		{
			user.AuthData.AccessFailedCount = 0;
			return Task.FromResult(0);
		}

		public virtual async Task<int> GetAccessFailedCountAsync(User user, CancellationToken token)
			=> user.AuthData.AccessFailedCount;

		public virtual async Task<bool> GetLockoutEnabledAsync(User user, CancellationToken token)
			=> user.AuthData.LockoutEnabled;

		public virtual async Task SetLockoutEnabledAsync(User user, bool enabled, CancellationToken token)
			=> user.AuthData.LockoutEnabled = enabled;

		public virtual IQueryable<User> Users => UserManager.MongoDatabase.AsQueryable();

		public virtual async Task SetTokenAsync(User user, string loginProvider, string name, string value, CancellationToken cancellationToken)
			=> user.AuthData.SetToken(loginProvider, name, value);

		public virtual async Task RemoveTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
			=> user.AuthData.RemoveToken(loginProvider, name);

		public virtual async Task<string> GetTokenAsync(User user, string loginProvider, string name, CancellationToken cancellationToken)
			=> user.AuthData.GetTokenValue(loginProvider, name);

		public void Dispose()
		{
		}
	}
}