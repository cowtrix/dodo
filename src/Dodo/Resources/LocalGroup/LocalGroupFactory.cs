using Resources;
using System;
using Common.Extensions;
using Common;
using Resources.Serializers;
using Dodo.DodoResources;
using Resources.Location;

namespace Dodo.LocalGroups
{
	public class LocalGroupSerializer : ResourceReferenceSerializer<LocalGroup> { }

	public class LocalGroupSchema : DescribedResourceSchemaBase
	{
		[View]
		public GeoLocation Location { get; set; }

		public LocalGroupSchema()
		{
		}

		public LocalGroupSchema(string name, string description, GeoLocation location) 
			: base(name, description)
		{
			Location = location;
		}
	}

	public class LocalGroupFactory : DodoResourceFactory<LocalGroup, LocalGroupSchema>
	{
	}
}
