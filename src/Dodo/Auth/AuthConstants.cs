using Common.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dodo
{
	public static class AuthConstants
	{
		public const string AUTHSCHEME = "Cookies";
		public const string KEY = "AuthToken";
		public const string SUBJECT = "sub";
	}
}
