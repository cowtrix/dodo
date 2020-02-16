using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;
using Resources;

namespace Dodo.Sites
{
	public class OccupationSiteSchema : SiteSchema
	{
		public OccupationSiteSchema()
		{
		}

		public OccupationSiteSchema(string name, string type, Guid parent, GeoLocation location, string description) : 
			base(name, type, parent, location, description)
		{
		}
	}

	public class OccupationSite : Site
	{
		public OccupationSite(AccessContext context, OccupationSiteSchema schema) : base(context, schema)
		{
		}
	}
}