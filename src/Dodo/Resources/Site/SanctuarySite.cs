using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;
using Resources;

namespace Dodo.Sites
{
	public class SanctuarySiteSchema : SiteSchema
	{
		public SanctuarySiteSchema(string name, string type, Guid parent, GeoLocation location, string description) : 
			base(name, type, parent, location, description)
		{
		}
	}
	public class SanctuarySite : Site
	{
		public SanctuarySite() : base(default, default) { }
		public SanctuarySite(AccessContext context, SanctuarySiteSchema schema) : base(context, schema)
		{
		}
	}
}