using REST;
using System;
using Common.Extensions;
using Common;
using REST.Serializers;

namespace Dodo.LocalGroups
{
	public class LocalGroupSerializer : ResourceReferenceSerializer<LocalGroup> { }

	public class LocalGroupSchema : GroupResourceSchemaBase
	{
		public GeoLocation Location { get; private set; }

		public LocalGroupSchema(AccessContext context, string name, GeoLocation location, string publicDescription, GroupResource parent) 
			: base(context, name, publicDescription, parent)
		{
			Location = location;
		}
	}

	public class LocalGroupFactory : ResourceFactory<LocalGroup, LocalGroupSchema>
	{
	}
}
