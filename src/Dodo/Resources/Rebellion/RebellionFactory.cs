using Common;
using Dodo.DodoResources;
using Resources;
using Resources.Location;
using Resources.Serializers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dodo.Rebellions
{
	public class RebellionSerializer : ResourceReferenceSerializer<Rebellion> { }

	public class RebellionSchema : DescribedResourceSchemaBase
	{
		[View]
		public GeoLocation Location { get; set; } = new GeoLocation();

		[View]
		[DataType(DataType.DateTime)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Constants.DateTimeFormat)]
		public DateTime StartDate { get; set; }

		[View]
		[DataType(DataType.DateTime)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Constants.DateTimeFormat)]
		public DateTime EndDate { get; set; }

		public RebellionSchema()
		{
		}

		public RebellionSchema(string name, string description, GeoLocation location, DateTime start, DateTime end)
			: base(name, description)
		{
			Location = location;
			StartDate = start;
			EndDate = end;
		}

		public override Type GetResourceType() => typeof(Rebellion);
	}

	public class RebellionFactory : DodoResourceFactory<Rebellion, RebellionSchema>
	{
	}
}
