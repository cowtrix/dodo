using Common;
using Dodo.Rebellions;
using Dodo.DodoResources;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.WorkingGroups
{
	public class WorkingGroupManager : DodoResourceManager<WorkingGroup>, ISearchableResourceManager
	{
	}
}
