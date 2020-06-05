using Common;
using System;
using Resources;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using Common.Extensions;
using System.ComponentModel.DataAnnotations;
using Resources.Location;

namespace Dodo.LocationResources
{
	public class EventSchema : LocationResourceSchema
	{
		[DataType(DataType.DateTime)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = Constants.DateTimeFormat)]
		public DateTime StartDate { get; set; }
		[DataType(DataType.DateTime)]
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

	[Name("Event")]
	public class Event : LocationResourceBase, ITimeBoundResource
	{
		public Event() : base() { }

		public Event(AccessContext context, EventSchema schema) : base(context, schema)
		{
		}

		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime StartDate 
		{ 
			get 
			{ 
				return __startDate; 
			} 
			set 
			{ 
				__startDate = value.ToUniversalTime(); 
				if (EndDate < StartDate) 
				{ 
					EndDate = StartDate + TimeSpan.FromHours(1); 
				} 
			} 
		}
		private DateTime __startDate;

		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime EndDate { get { return __endDate; } set { __endDate = value.ToUniversalTime(); } }
		private DateTime __endDate;

		public override bool VerifyExplicit(out string error)
		{
			if (EndDate < StartDate)
			{
				error = "End date cannot be earlier than start date";
				return false;
			}
			return base.VerifyExplicit(out error);
		}
	}
}
