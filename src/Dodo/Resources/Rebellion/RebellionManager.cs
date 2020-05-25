using Common;
using Dodo.Resources;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.Rebellions
{
	public class RebellionManager : DodoResourceManager<Rebellion>, ISearchableResourceManager
	{
	}
}
