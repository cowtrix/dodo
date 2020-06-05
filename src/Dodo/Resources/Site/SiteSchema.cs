using Resources.Location;
using System;

namespace Dodo.LocationResources
{
	public abstract class LocationResourceSchema : OwnedResourceSchemaBase
	{
		public GeoLocation Location { get; set; }

		public LocationResourceSchema(string name, Guid parent, GeoLocation location, string description)
			: base(name, description, parent)
		{
			Location = location;
		}

		public LocationResourceSchema()
		{
		}
	}
}
