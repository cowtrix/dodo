using Common;
using Dodo.Rebellions;
using Dodo.DodoResources;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.Roles
{
	public class RoleManager : DodoResourceManager<Role>, ISearchableResourceManager
	{
	}
}
