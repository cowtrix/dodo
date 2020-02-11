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

		public static AccessContext GetRequestOwner(this ClaimsPrincipal request)
		{
			var username = request.FindFirst(AuthService.USERNAME)?.Value;
			if(string.IsNullOrEmpty(username))
			{
				return default;
			}
			var user = ResourceUtility.GetManager<User>().GetSingle(u => u.AuthData.Username == username);
			return new AccessContext(user, "");
		}
	}
}