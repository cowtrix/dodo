using Common;
using Common.Extensions;
using Dodo.Resources;
using Resources;
using Resources.Serializers;
using System;

namespace Dodo.Rebellions
{
	public class RebellionSerializer : ResourceReferenceSerializer<Rebellion> { }

	public class RebellionSchema : GroupResourceSchemaBase
	{
		public GeoLocation Location { get; private set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }

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
