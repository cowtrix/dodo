using Common.Extensions;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using REST;
using REST.Security;

namespace Dodo.Utility
{
	public static class DodoHttpExtensions
	{
		private const string AUTH_KEY = "Authorization";

		public static AccessContext TryGetRequestOwner(this HttpRequest request)
		{
			try
			{
				return GetRequestOwner(request);
			}
			catch (HttpException)
			{ }
			return default;
		}

		/// <summary>
		/// Get the user that made an HTTP request, validate authentication, and return the
		/// unlocked passphrase. DO NOT store this passphrase anywhere except as a local
		/// scope variable.
		/// </summary>
		/// <param name="request">The requ</param>
		/// <returns>The user context that made this request</returns>
		public static AccessContext GetRequestOwner(this HttpRequest request)
		{
			GetAuth(request, out var username, out var password);
			if (username == null || password == null)
			{
				return default;
			}
			var user = ResourceUtility.GetManager<User>().GetSingle(x => x.WebAuth.Username == username);
			if (user != null && !user.WebAuth.ChallengePassword(password, out var passphrase))
			{
				throw HttpException.FORBIDDEN;
			}
			return new AccessContext(user, passphrase);
		}

		public static void GetAuth(this HttpRequest request, out string username, out string password)
		{
			username = null;
			password = null;
			if (!request.Headers.TryGetValue(AUTH_KEY, out var token))
			{
				return;
			}
			var tokens = token.ToString().Trim().Split(' ');
			if (tokens.Length != 2 || tokens[0] != "Basic")
			{
				throw HttpException.UNAUTHORIZED;
			}
			var decodeRaw = StringExtensions.Base64Decode(tokens[1]);
			var firstColonIndex = decodeRaw.IndexOf(':');
			if (firstColonIndex == 0)
			{
				// No auth but header existed
				return;
			}
			if (firstColonIndex < 0)
			{
				throw new HttpException("Bad Auth Header Format", System.Net.HttpStatusCode.BadRequest);
			}
			username = decodeRaw.Substring(0, firstColonIndex);
			password = decodeRaw.Substring(firstColonIndex + 1);
		}
	}
}