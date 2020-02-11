using Common.Extensions;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using REST;
using REST.Security;
using System;
using System.Security.Claims;

namespace Dodo.Utility
{
	public static class DodoHttpExtensions
	{
		private const string AUTH_KEY = "Authorization";

		/// <summary>
		/// Get the user that made an HTTP request, validate authentication, and return the
		/// unlocked passphrase. DO NOT store this passphrase anywhere except as a local
		/// scope variable.
		/// </summary>
		/// <param name="request">The requ</param>
		/// <returns>The user context that made this request</returns>
		public static AccessContext GetRequestOwner(this ClaimsPrincipal request)
		{
			var username = request.FindFirst(AuthService.USERNAME).Value;
			var user = ResourceUtility.GetManager<User>().GetSingle(u => u.AuthData.Username == username);
			return new AccessContext(user, "");
		}
	}
}