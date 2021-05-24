using Common;
using System;
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
		[Name("Start Date")]
		//[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Constants.DateTimeFormat)]
		public DateTime StartDate { get; set; }

		[View]
		[DataType(DataType.DateTime)]
		[Name("End Date")]
		//[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Constants.DateTimeFormat)]
		public DateTime EndDate { get; set; }

		public EventSchema()
		{
		}

		public EventSchema(string name, string parent, GeoLocation location, string description, SiteFacilities facilities, string videoEmbedURL, DateTime start, DateTime end)
			: base(name, parent, location, description, facilities, videoEmbedURL)
		{
			Location = location;
			StartDate = start;
			EndDate = end;
		}

		public override Type GetResourceType() => typeof(Event);
	}
}
