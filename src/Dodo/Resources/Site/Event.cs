using Common;
using System;
using Resources;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;

namespace Dodo.Sites
{
	[Name("Event")]
	public class EventSite : Site, ITimeBoundResource
	{
		public EventSite() : base() { }

		public EventSite(AccessContext context, SiteSchema schema) : base(context, schema)
		{
		}

		[JsonProperty]
		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime StartDate { get; set; }

		[JsonProperty]
		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime EndDate { get; set; }
	}
}
