using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;
using Resources;

namespace Dodo.Sites
{
	public class SanctuarySite : Site
	{
		public SanctuarySite() : base(default, default) { }
		public SanctuarySite(AccessContext context, SiteSchema schema) : base(context, schema)
		{
		}
	}
}