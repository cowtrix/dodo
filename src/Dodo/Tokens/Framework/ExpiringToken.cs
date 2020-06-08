using Common.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace Dodo.Users.Tokens
{
	public abstract class ExpiringToken : Token, IRemovableToken
	{
		[JsonIgnore]
		[BsonIgnore]
		public bool ShouldRemove => DateTime.Now > ExpiryDate;
		[JsonProperty]
		[BsonElement]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime ExpiryDate { get; set; }

		public abstract void OnRemove(AccessContext parent);

		public ExpiringToken() { }

		public ExpiringToken(DateTime expiry) : base()
		{
			ExpiryDate = expiry.Trim(TimeSpan.TicksPerMinute).ToUniversalTime();
		}
	}

}
