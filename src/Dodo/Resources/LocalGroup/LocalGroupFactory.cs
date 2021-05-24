using Resources;
using System;
using Resources.Serializers;
using Dodo.DodoResources;
using Resources.Location;

namespace Dodo.LocalGroups
{
	public class LocalGroupSerializer : ResourceReferenceSerializer<LocalGroup> { }

	public class LocalGroupSchema : DescribedResourceSchemaBase
	{
		[View]
		public GeoLocation Location { get; set; } = new GeoLocation();

		public LocalGroupSchema()
		{
		}

		public LocalGroupSchema(string name, string description, GeoLocation location) 
			: base(name, description)
		{
			Location = location;
		}

		public override Type GetResourceType() => typeof(LocalGroup);
	}

	public class LocalGroupFactory : DodoResourceFactory<LocalGroup, LocalGroupSchema>
	{
	}
}
