using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;
using Resources;

namespace Dodo.Sites
{
	public class PermanentSite : Site
	{
		public PermanentSite() : base() { }
		public PermanentSite(AccessContext context, SiteSchema schema) : base(context, schema)
		{
		}
	}
}
