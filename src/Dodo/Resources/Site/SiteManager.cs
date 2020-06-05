using Common;
using Dodo.Rebellions;
using Dodo.Resources;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.LocationResources
{
	public class EventManager : DodoResourceManager<Event>, ISearchableResourceManager
	{
	}

	public class SiteManager : DodoResourceManager<Site>, ISearchableResourceManager
	{
	}
}
