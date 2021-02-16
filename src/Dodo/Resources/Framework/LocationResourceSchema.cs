using Resources;
using Resources.Location;
using System;

namespace Dodo.LocationResources
{
	public abstract class LocationResourceSchema : OwnedResourceSchemaBase
	{
		[View]
		public string VideoEmbedURL;
		[View]
		public SiteFacilities Facilities;
		[View]
		public GeoLocation Location { get; set; } = new GeoLocation();

		public LocationResourceSchema(string name, string parent, GeoLocation location, string description, SiteFacilities facilities, string videoEmbedURL)
			: base(name, description, parent)
		{
			Location = location;
			Facilities = facilities;
			VideoEmbedURL = videoEmbedURL;
		}

		public LocationResourceSchema()
		{
		}
	}
}
