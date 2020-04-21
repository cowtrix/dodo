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
		public List<Guid> WorkingGroups = new List<Guid>();
		[View(EPermissionLevel.PUBLIC)]
		public List<Guid> Sites = new List<Guid>();

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
			if (type == typeof(WorkingGroup) || type == typeof(Site))
			{
				return true;
			}
			return false;
		}

		public override void AddChild<T>(T rsc)
		{
			if (rsc is WorkingGroup wg && wg.Parent.Guid == Guid)
			{
				if(WorkingGroups.Contains(wg.Guid))
				{
					throw new Exception($"Adding duplicated child object {wg.Guid} to {Guid}");
				}
				WorkingGroups.Add(wg.Guid);
			}
			else if (rsc is Site s && s.Parent.Guid == Guid)
			{
				if (Sites.Contains(s.Guid))
				{
					throw new Exception($"Adding duplicated child object {s.Guid} to {Guid}");
				}
				Sites.Add(s.Guid);
			}
			else
			{
				throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
			}
		}

		public override bool RemoveChild<T>(T rsc)
		{
			if (rsc is WorkingGroup wg && wg.Parent.Guid == Guid)
			{
				return WorkingGroups.Remove(wg.Guid);
			}
			else if (rsc is Site s && s.Parent.Guid == Guid)
			{
				return Sites.Remove(s.Guid);
			}
			throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
		}
	}
}
