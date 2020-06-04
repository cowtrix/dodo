using Common;
using System;
using Resources;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using Common.Extensions;

namespace Dodo.Sites
{
	[Name("Event")]
	public class EventSite : Site, ITimeBoundResource
	{
		public EventSite() : base() { }

		public EventSite(AccessContext context, SiteSchema schema) : base(context, schema)
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
