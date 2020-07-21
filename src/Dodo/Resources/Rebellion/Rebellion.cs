using Common;
using Common.Extensions;
using Resources.Security;
using Dodo.LocationResources;
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
using Dodo.Users.Tokens;

namespace Dodo.Rebellions
{
	[Name("Rebellion")]
	[SearchPriority(0)]
	public class Rebellion : AdministratedGroupResource, ILocationalResource, ITimeBoundResource, IVideoResource
	{
		public const string ROOT = "rebellions";

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		[Name("Working Groups")]
		public List<ResourceReference<WorkingGroup>> WorkingGroups { get; set; } = new List<ResourceReference<WorkingGroup>>();
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public List<ResourceReference<Site>> Sites { get; set; } = new List<ResourceReference<Site>>();
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public List<ResourceReference<Event>> Events { get; set; } = new List<ResourceReference<Event>>();

		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		[Name("Start Date")]
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
		[Name("End Date")]
		public DateTime EndDate { get { return __endDate; } set { __endDate = value.ToUniversalTime(); } }
		private DateTime __endDate;

		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; set; } = new GeoLocation();

		[View(EPermissionLevel.PUBLIC)]
		[Name("Banner Video Embed URL")]
		public string VideoEmbedURL { get; set; }

		public Rebellion() : base() { }

		public Rebellion(AccessContext context, RebellionSchema schema) : base(context, schema)
		{
			Location = schema.Location;
			StartDate = schema.StartDate;
			EndDate = schema.EndDate;
		}

		public override bool CanContain(Type type)
		{
			if (type == typeof(WorkingGroup) || type == typeof(LocationResourceBase))
			{
				return true;
			}
			return false;
		}

		public override void AddChild<T>(T rsc)
		{
			if (rsc is WorkingGroup wg && wg.Parent.Guid == Guid)
			{
				if(WorkingGroups.Any(w => w.Guid == wg.Guid))
				{
					throw new Exception($"Adding duplicated child object {wg.Guid} to {Guid}");
				}
				WorkingGroups.Add(wg.CreateRef());
			}
			else if (rsc is Site s && s.Parent.Guid == Guid)
			{
				if (Sites.Any(w => w.Guid == s.Guid))
				{
					throw new Exception($"Adding duplicated child object {s.Guid} to {Guid}");
				}
				Sites.Add(s.CreateRef());
			}
			else if (rsc is Event e && e.Parent.Guid == Guid)
			{
				if (Events.Any(w => w.Guid == e.Guid))
				{
					throw new Exception($"Adding duplicated child object {e.Guid} to {Guid}");
				}
				Events.Add(e.CreateRef());
			}
			else
			{
				throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
			}
			base.AddChild(rsc);
		}

		public override bool RemoveChild<T>(T rsc)
		{
			if (rsc is WorkingGroup wg && wg.Parent.Guid == Guid)
			{
				return WorkingGroups.Remove(wg.CreateRef()) && base.RemoveChild(rsc);
			}
			else if (rsc is Site s && s.Parent.Guid == Guid)
			{
				return Sites.Remove(s.CreateRef()) && base.RemoveChild(rsc);
			}
			else if (rsc is Event e && e.Parent.Guid == Guid)
			{
				return Events.Remove(e.CreateRef()) && base.RemoveChild(rsc);
			}
			throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
		}
	}
}
