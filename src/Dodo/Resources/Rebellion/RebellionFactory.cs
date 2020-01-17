using Common;
using Common.Extensions;
using REST;
using REST.Serializers;
using System;

namespace Dodo.Rebellions
{
	public class RebellionSerializer : ResourceReferenceSerializer<Rebellion> { }

	public class RebellionSchema : GroupResourceSchemaBase
	{
		public GeoLocation Location { get; private set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

		public RebellionSchema(AccessContext context, string name, GeoLocation location, string publicDescription, GroupResource parent, DateTime startDate, DateTime endDate)
			: base(context, name, publicDescription, parent)
		{
			Location = location;
			StartDate = startDate;
			EndDate = EndDate;
		}
	}

	public class RebellionFactory : ResourceFactory<Rebellion, RebellionSchema>
	{
	}
}
