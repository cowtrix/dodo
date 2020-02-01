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
			username = request.HttpContext.User.Identity.Name;
			password = (string)request.HttpContext.Items["Passphrase"];
		}
	}
}