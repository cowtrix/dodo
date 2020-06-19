using Common;
using Common.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace Resources.Location
{
	public class LocationData
	{
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public string Country { get; set; }
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public string Region { get; set; }
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public string Postcode { get; set; }
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public string District { get; set; }
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public string Place { get; set; }
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public string Locality { get; set; }
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public string Neighborhood { get; set; }
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public string Address { get; set; }
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public string TimezoneID { get; set; }

		[BsonIgnore]
		[JsonIgnore]
		public bool IsEmpty
		{
			get
			{
				return Address == null &&
					Neighborhood == null &&
					Locality == null &&
					Place == null &&
					District == null &&
					Postcode == null &&
					Region == null &&
					Country == null;
			}
		}

		[BsonIgnore]
		[JsonIgnore]
		public TimeZoneInfo Timezone
		{
			get
			{
				try
				{
					if(string.IsNullOrEmpty(TimezoneID))
					{
						return TimeZoneInfo.Utc;
					}
					return TimeZoneInfo.FindSystemTimeZoneById(TimezoneID);
				}
				catch(Exception e)
				{
					Logger.Exception(e);
					return TimeZoneInfo.Utc;
				}
			}
		}

		public override string ToString() =>
			Address.AppendIfNotNull(", ") +
			Neighborhood.AppendIfNotNull(", ") +
			Locality.AppendIfNotNull(", ") +
			Place.AppendIfNotNull(", ") +
			District.AppendIfNotNull(", ") +
			Postcode.AppendIfNotNull(", ") +
			Region.AppendIfNotNull(", ") +
			Country;
	}
}
