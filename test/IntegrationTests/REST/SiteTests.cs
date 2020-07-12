using Dodo;
using Dodo.LocationResources;
using Dodo.Sites;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using Resources;

namespace RESTTests
{
	public abstract class SiteTests<T> : RESTTestBase<T, LocationResourceSchema> where T:LocationResourceBase
	{
	}
}
