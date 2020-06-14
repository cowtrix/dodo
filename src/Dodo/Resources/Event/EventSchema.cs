using Common;
using System;
using Newtonsoft.Json;
using Common.Extensions;
using System.ComponentModel.DataAnnotations;
using Resources.Location;
using Resources;
using System.ComponentModel;

namespace Dodo.LocationResources
{
	public class EventSchema : LocationResourceSchema
	{
		[View]
		[DataType(DataType.DateTime)]
		[DisplayName("Start Date")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Constants.DateTimeFormat)]
		public DateTime StartDate { get; set; }

		[View]
		[DataType(DataType.DateTime)]
		[DisplayName("End Date")]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Constants.DateTimeFormat)]
		public DateTime EndDate { get; set; }

		public EventSchema()
		{
		}

		public EventSchema(string name, Guid parent, GeoLocation location, string description, DateTime start, DateTime end)
			: base(name, parent, location, description)
		{
			Location = location;
			StartDate = start;
			EndDate = end;
		}
	}
}
