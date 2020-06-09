using Common;
using Dodo.DodoResources;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.LocalGroups
{
	public class LocalGroupManager : DodoResourceManager<LocalGroup>, ISearchableResourceManager
	{
	}
}
