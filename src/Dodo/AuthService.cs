using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using REST;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Dodo
{

    public class AuthService : IAuthorizationService
	{
		public const string PERMISSION_LEVEL = "PERMISSION";
		public const string USERNAME = "USERNAME";

		public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
		{
			return AuthorizationResult.Success();
			throw new NotImplementedException();
		}

		public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
		{
			throw new NotImplementedException();
		}
	}
}
