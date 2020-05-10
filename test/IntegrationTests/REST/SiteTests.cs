using Dodo;
using Dodo.Sites;
using DodoResources.Sites;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using Resources;

namespace RESTTests
{
	public abstract class SiteTests<T> : RESTTestBase<T, SiteSchema> where T:Site
	{
		public override string ResourceRoot => SiteController.RootURL;
		protected override string PostmanCategory => "Sites";
	}
}
