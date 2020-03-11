using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;
using Resources;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;

namespace Dodo.Sites
{
	public class EventSite : Site, ITimeBoundResource
	{
		public EventSite() : base(default, default) { }

		public EventSite(AccessContext context, SiteSchema schema) : base(context, schema)
		{
		}

		[JsonProperty]
		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime StartDate { get; set; }

		[JsonProperty]
		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime EndDate { get; set; }
	}
}
