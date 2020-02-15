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
    public class AuthService : DefaultAuthorizationService
	{
		public const string PERMISSION_LEVEL = "PERMISSION";
		public const string USERNAME = "USERNAME";

		public AuthService(IAuthorizationPolicyProvider policyProvider, IAuthorizationHandlerProvider handlers, ILogger<DefaultAuthorizationService> logger, IAuthorizationHandlerContextFactory contextFactory, IAuthorizationEvaluator evaluator, IOptions<AuthorizationOptions> options) : base(policyProvider, handlers, logger, contextFactory, evaluator, options)
		{
		}

		public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, IEnumerable<IAuthorizationRequirement> requirements)
		{
			return await base.AuthorizeAsync(user, resource, requirements);
		}

		public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object resource, string policyName)
		{
			throw new NotImplementedException();
		}
	}
}
