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

		public SiteSchema(string name, string parent, GeoLocation location, string description, SiteFacilities facilities, string videoEmbedURL)
			: base(name, parent, location, description, facilities, videoEmbedURL)
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
