using Common;
using Resources.Location;
using System;

namespace Dodo.LocationResources
{
	public class SiteSchema : LocationResourceSchema
	{
		public SiteSchema()
		{
		}

		public SiteSchema(string name, Guid parent, GeoLocation location, string description)
			: base(name, parent, location, description)
		{
		}
	}

	[Name("Site")]
	public class Site : LocationResourceBase
	{
		public Site() : base() { }
		public Site(AccessContext context, LocationResourceSchema schema) : base(context, schema)
		{
		}
	}
}
