using Common;
using Common.Extensions;
using Dodo.Resources;
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

		public RebellionSchema(AccessContext context, string name, GeoLocation location, string publicDescription, DateTime startDate, DateTime endDate)
			: base(context, name, publicDescription, null)
		{
			Location = location;
			StartDate = startDate;
			EndDate = endDate;
		}

		public RebellionSchema()
		{
		}

		public RebellionSchema(string name, string description, GeoLocation location, DateTime start, DateTime end)
			: base(name, description, default)
		{
			Location = location;
			StartDate = start;
			EndDate = end;
		}
	}

	public class RebellionFactory : DodoResourceFactory<Rebellion, RebellionSchema>
	{
	}
}
