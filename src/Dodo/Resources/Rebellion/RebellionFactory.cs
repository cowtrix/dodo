using Common;
using Common.Extensions;
using Dodo.DodoResources;
using Resources;
using Resources.Location;
using Resources.Serializers;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dodo.Rebellions
{
	public class RebellionSerializer : ResourceReferenceSerializer<Rebellion> { }

	public class RebellionSchema : OwnedResourceSchemaBase
	{
		[View]
		public GeoLocation Location { get; set; }

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
