using Dodo.Users;
using Resources;
using Resources.Security;
using System;
using System.Security.Claims;

namespace Dodo
{
	public static class AuthExtensions
	{
		private static readonly IResourceManager<User> m_userManager;

		static AuthExtensions()
		{
			m_userManager = ResourceUtility.GetManager<User>();
		}

		public static AccessContext GetContext(this ClaimsPrincipal claims)
		{
			if (claims.Identity == null || !claims.Identity.IsAuthenticated)
			{
				return default;
			}
			if (!(claims.Identity is ClaimsIdentity claimsID))
			{
				return default;
			}

			var guidKey = claimsID.FindFirst(AuthConstants.SUBJECT).Value;
			if (!TemporaryTokenManager.CheckToken(guidKey, out var guidStr)
				|| !Guid.TryParse(guidStr, out var userGuid))
			{
				return default;
			}
			var user = m_userManager.GetSingle(x => x.GUID == userGuid);

			var tokenKey = claimsID.FindFirst(AuthConstants.KEY).Value;
			if (!TemporaryTokenManager.CheckToken(tokenKey, out var passphrase))
			{
				return default;
			}
			return new AccessContext(user, passphrase);
		}
	}
}
