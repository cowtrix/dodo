using Common.Extensions;
using REST;
using Microsoft.AspNetCore.Http;
using System;
using Dodo.Utility;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Dodo.Roles
{
	[Route(RootURL)]
	public class RoleController : ObjectRESTController<Role, RoleSchema>
	{
		public const string RootURL = "api/roles";
	}
}
