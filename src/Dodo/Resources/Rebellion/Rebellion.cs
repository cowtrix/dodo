using Common;
using Common.Extensions;
using Resources.Security;
using Dodo.Sites;
using Dodo.Users;
using Dodo.WorkingGroups;
using Resources;
using Resources.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using Resources.Location;

namespace Dodo.Rebellions
{
	[Name("Rebellion")]
	public class Rebellion : GroupResource, ILocationalResource, ITimeBoundResource
	{
		public const string ROOT = "rebellions";

		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; set; }

		[View(EPermissionLevel.PUBLIC)]
		public List<string> WorkingGroups
		{
			get
			{
				return ResourceUtility.GetManager<WorkingGroup>().Get(wg => wg.IsChildOf(this)).Select(x => x.Guid.ToString()).ToList();
			}
		}

		[View(EPermissionLevel.PUBLIC)]
		public List<string> Sites
		{
			get
			{
				return ResourceUtility.GetManager<Site>().Get(wg => wg.Parent.Guid == Guid).Select(x => x.Guid.ToString()).ToList();
			}
		}

		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime StartDate { get; set; }

		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime EndDate { get; set; }

		public Rebellion(AccessContext context, RebellionSchema schema) : base(context, schema)
		{
			Location = schema.Location;
			StartDate = schema.StartDate;
			EndDate = schema.EndDate;
		}

		public override bool CanContain(Type type)
		{
			if(type == typeof(WorkingGroup) || type == typeof(Site))
			{
				return true;
			}
			return false;
		}
	}
}
