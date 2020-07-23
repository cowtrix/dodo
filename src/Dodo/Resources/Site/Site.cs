using Common;
using Resources;
using Resources.Location;
using System;

namespace Dodo.LocationResources
{
	public class SiteSchema : LocationResourceSchema
	{
		public SiteSchema()
		{
		}

		public SiteSchema(string name, string parent, GeoLocation location, string description)
			: base(name, parent, location, description)
		{
		}

		public override Type GetResourceType() => typeof(Site);
	}

	[Name("Site")]
	[SearchPriority(2)]
	public class Site : LocationResourceBase
	{
		public Site() : base() { }
		public Site(AccessContext context, LocationResourceSchema schema) : base(context, schema)
		{
		}
	}
}
