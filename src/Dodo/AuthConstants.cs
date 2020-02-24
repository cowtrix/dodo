using Common.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Resources;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dodo
{
	public static class AuthConstants
	{
		public const string AUTHSCHEME = "idsrv";
		public const string KEY = "AuthToken";
		public const string Subject = "sub";
	}

	public class AuthService : IAuthorizationService
	{
		public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
		{
			return AuthorizationResult.Success();
		}

		public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
		{
			return AuthorizationResult.Success();
		}
	}
}
