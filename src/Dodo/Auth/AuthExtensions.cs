using Common;
using Dodo.Security;
using Dodo.Users;
using Dodo.Users.Tokens;
using Resources;
using Resources.Security;
using System;
using System.Linq;
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
			// Make sure we have the info needed
			if (claims == null || claims.Identity == null || !claims.Identity.IsAuthenticated)
			{
				return default;
			}
			if (!(claims.Identity is ClaimsIdentity claimsID))
			{
				return default;
			}

			// The claim has a token that can be used to find the user it is for
			// and a password to decrypt that content
			var userToken = claimsID.FindFirst(AuthConstants.SUBJECT).Value;
			var sessionKey = claimsID.FindFirst(AuthConstants.KEY).Value;

			var user = SessionTokenStore.GetUser(userToken, sessionKey);
			if(user == null)
			{
				return default;
			}

			var sessionToken = user.TokenCollection.GetAllTokens<SessionToken>(default)
				.SingleOrDefault(t => t?.UserKey == userToken);
			if(sessionToken == null)
			{
				Logger.Warning($"Session token was null but user had valid user token - shouldn't really happen.");
				return default;
			}
			var passphrase = sessionToken.EncryptedPassphrase.GetValue(sessionKey);

			// Create the context and make sure its valid
			var context = new AccessContext(user, passphrase, userToken);
			if(!context.Challenge())
			{
				return default;
			}
			return context;
		}
	}
}
