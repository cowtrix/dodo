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
	public static class AuthService
	{
		public static string JwtKey => new ConfigVariable<string>("JWTSecret", 
			"401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1").Value;
		public const string GUID = "USERNAME";
		public const string JWTHEADER = "Bearer";
		public const string KEY = "AuthToken";
	}
}
