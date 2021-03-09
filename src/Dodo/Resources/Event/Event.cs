using Common;
using System;
using Resources;
using MongoDB.Bson.Serialization.Attributes;
using Resources.Security;
using Dodo.Email;

namespace Dodo.LocationResources
{
	[Name("Event")]
	[SearchPriority(2)]
	public class Event : LocationResourceBase, ITimeBoundResource
	{
		public Event() : base() { }

		public Event(AccessContext context, EventSchema schema) : base(context, schema)
		{
			StartDate = schema.StartDate;
			EndDate = schema.EndDate;
		}

		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		[Name("Start Date")]
		[PatchCallback(nameof(OnValueChanged))]
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
		[BsonElement]
		private DateTime __startDate;

		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		[Name("End Date")]
		[PatchCallback(nameof(OnValueChanged))]
		public DateTime EndDate { get { return __endDate; } set { __endDate = value.ToUniversalTime(); } }
		[BsonElement]
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

		public override bool CanContain(Type type) => false;
	}
}
