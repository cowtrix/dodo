using Resources;
using Resources.Location;
using System;

namespace Dodo.LocationResources
{
	public abstract class LocationResourceSchema : OwnedResourceSchemaBase
	{
		[View]
		public GeoLocation Location { get; set; } = new GeoLocation();

		public LocationResourceSchema(string name, string parent, GeoLocation location, string description)
			: base(name, description, parent)
		{
			Location = location;
		}

		public LocationResourceSchema()
		{
		}
	}
}
